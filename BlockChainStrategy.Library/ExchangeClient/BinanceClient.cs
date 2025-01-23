using BlockChainStrategy.Library.Enums.Binance;
using BlockChainStrategy.Library.Models.Dto.Binance;
using BlockChainStrategy.Library.Models.Dto.Binance.Event;
using BlockChainStrategy.Library.Models.Dto.Binance.Request;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;

namespace BlockChainStrategy.Library.Exchange
{
    public class BinanceClient : IDisposable, IExchangeClient
    {
        private string _baseUrl;
        private string _wsUrl;
        private string _apiKey;
        private string _apiSecret;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cts;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, TaskCompletionSource<string>> _orderTcsMap = new Dictionary<string, TaskCompletionSource<string>>();
        private readonly Dictionary<string, BinanceTradeLiteEvent> _oderListenInfo = new Dictionary<string, BinanceTradeLiteEvent>();

        public BinanceClient(string apiKey, string apiSecret, bool test = false)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _baseUrl = test ? "https://testnet.binancefuture.com/fapi/v1" : "https://fapi.binance.com/fapi/v1";
            _wsUrl = test ? "wss://fstream.binancefuture.com/ws" : "wss://fstream.binance.com";
        }

        /// <summary>
        /// 初始化WebSocket，開始監聽User Data Stream
        /// </summary>
        public async Task ListenWebSocketAsync()
        {
            var listenKey = await CreateUserDataStreamAsync();

            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            var wsUri = new Uri($"{_wsUrl}/{listenKey}");
            await _webSocket.ConnectAsync(wsUri, _cts.Token);
            Console.WriteLine("Binance WS Connected!");

            _ = Task.Run(ReceiveLoopAsync);
        }

        /// <summary>
        /// 下單並(可選)等待該訂單最終狀態
        /// </summary>
        public async Task<OrderResponse?> CreateOrderAsync(OrderRequest request, bool waitFinalStatus = true)
        {
            var binanceRequest = new BinanceCreateOrderRequestDto()
            {
                Symbol = request.Symbol,
                Side = request.Side,
                Quantity = request.Quantity,
                NewClientOrderId = "Binance_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_" + Guid.NewGuid()
            };

            TaskCompletionSource<string>? tcs = null;
            if (waitFinalStatus)
            {
                tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                _orderTcsMap[binanceRequest.NewClientOrderId] = tcs;
            }

            var response = await SendCreateOrderRequest(binanceRequest);

            if (!waitFinalStatus)
            {
                return new OrderResponse();
                //return response;
            }
            else
            {
                try
                {
                    var finalStatus = await tcs!.Task;
                    Console.WriteLine($"訂單 {binanceRequest.NewClientOrderId} 狀態: {finalStatus}");
                    var listenInfo = _oderListenInfo[binanceRequest.NewClientOrderId];

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"等待訂單失敗: {ex.Message}");
                }
                finally
                {
                    _orderTcsMap.Remove(binanceRequest.NewClientOrderId);
                    _oderListenInfo.Remove(binanceRequest.NewClientOrderId);
                }
                return new OrderResponse();
            }
        }

        /// <summary>
        /// REST: 建立UserDataStream (取得 listenKey)
        /// </summary>
        private async Task<string> CreateUserDataStreamAsync()
        {
            string endpoint = "/listenKey";
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{endpoint}");
            req.Headers.Add("X-MBX-APIKEY", _apiKey);

            var res = await _httpClient.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var listenKeyDto = JsonConvert.DeserializeObject<BinanceListenKeyResponseDto>(content);
            return listenKeyDto?.ListenKey ?? throw new Exception("Failed to get listenKey");
        }

        /// <summary>
        /// REST: 送出下單請求
        /// </summary>
        private async Task<BinanceCreateOrderResponseDto?> SendCreateOrderRequest(BinanceCreateOrderRequestDto request)
        {
            string endpoint = "/order";
            string url = $"{_baseUrl}{endpoint}";

            var param = new Dictionary<string, string>
            {
                { "symbol", request.Symbol },
                { "side", request.Side.ToString() },
                { "positionSide", request.PositionSide.ToString() },
                { "type", request.Type.ToString() },
                { "quantity", request.Quantity.ToString() },
                { "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
            };

            if (!string.IsNullOrEmpty(request.NewClientOrderId)) 
                param["newClientOrderId"] = request.NewClientOrderId;

            if (request.Type == OrderType.LIMIT)
            {
                if(request.Price == 0) throw new Exception("Price is required for LIMIT order");
                param["price"] = request.Price.ToString();
                param["timeInForce"] = request.TimeInForce ?? "GTC";
            }

            var queryString = GetQueryString(param);
            var signature = GenerateSignature(queryString);
            param.Add("signature", signature);

            var reqMsg = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(param)
            };
            reqMsg.Headers.Add("X-MBX-APIKEY", _apiKey);

            var resp = await _httpClient.SendAsync(reqMsg);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"CreateOrder error: {error}");
            }
            var resContent = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BinanceCreateOrderResponseDto>(resContent);
        }

        /// <summary>
        /// WebSocket 接收迴圈
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            if (_webSocket == null) return;
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open && _cts?.IsCancellationRequested == false)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Binance WS closed!");
                        break;
                    }
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    HandleMessage(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Binance WS Error: {ex.Message}");
                    break;
                }
            }
        }

        private void HandleMessage(string json)
        {
            var jObject = JObject.Parse(json);
            var eventType = jObject["e"]?.ToString();
            if (Enum.TryParse<EventStatus>(eventType, true, out var status))
            {
                switch (status)
                {
                    case EventStatus.TRADE_LITE:
                        var tradeLiteResult = JsonConvert.DeserializeObject<BinanceTradeLiteEvent>(json);
                        Console.WriteLine($"[TRADE_LITE]: {JsonConvert.SerializeObject(tradeLiteResult)}");
                        var tradeClientOrderId = tradeLiteResult?.ClientOrderId;
                        if (!string.IsNullOrEmpty(tradeClientOrderId) && _orderTcsMap.TryGetValue(tradeClientOrderId, out var tradeTcs))
                        {
                            if (!_oderListenInfo.TryAdd(tradeClientOrderId, tradeLiteResult!))
                            {
                                _oderListenInfo[tradeClientOrderId] = tradeLiteResult!;
                            }

                            tradeTcs.TrySetResult("FILLED");
                        }
                        break;

                    case EventStatus.ACCOUNT_UPDATE:
                        var accountResult = JsonConvert.DeserializeObject<BinanceAccountUpdateEvent>(json);
                        Console.WriteLine($"[ACCOUNT_UPDATE]: {JsonConvert.SerializeObject(accountResult)}");
                        break;

                    default:
                        Console.WriteLine($"[UNKNOWN_EVENT_TYPE]: {eventType}, JSON: {json}");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"[INVALID_EVENT_TYPE]: {eventType}, JSON: {json}");
            }
        }

        #region Utilities

        private string GetQueryString(Dictionary<string, string> param)
        {
            var sb = new StringBuilder();
            foreach (var p in param)
            {
                if (!string.IsNullOrEmpty(p.Value))
                {
                    sb.Append($"{p.Key}={Uri.EscapeDataString(p.Value)}&");
                }
            }
            return sb.ToString().TrimEnd('&');
        }

        private string GenerateSignature(string queryString)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public void Dispose()
        {
            try
            {
                _cts?.Cancel();
                _webSocket?.Dispose();
            }
            catch { }
        }
        #endregion
    }
}
