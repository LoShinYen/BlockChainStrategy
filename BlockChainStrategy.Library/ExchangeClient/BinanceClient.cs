using BlockChainStrategy.Library.Enums.Binance;
using BlockChainStrategy.Library.Helpers;
using BlockChainStrategy.Library.Models.Dto.Binance;
using BlockChainStrategy.Library.Models.Dto.Binance.Event;
using BlockChainStrategy.Library.Models.Dto.Binance.Request;
using BlockChainStrategy.Library.Models.Dto.Binance.Response;
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
        private string _apiKey = string.Empty;
        private string _apiSecret = string.Empty;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cts;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Dictionary<string, TaskCompletionSource<string>> _orderTcsMap = new Dictionary<string, TaskCompletionSource<string>>();
        private readonly Dictionary<string, BinanceTradeLiteEvent> _oderListenInfo = new Dictionary<string, BinanceTradeLiteEvent>();

        public BinanceClient(bool test = false)
        {
            _baseUrl = test ? "https://testnet.binancefuture.com/fapi/v1" : "https://fapi.binance.com/fapi/v1";
            _wsUrl = test ? "wss://fstream.binancefuture.com/ws" : "wss://fstream.binance.com";
        }

        public BinanceClient(string apiKey, string apiSecret, bool test = false)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _baseUrl = test ? "https://testnet.binancefuture.com/fapi/v1" : "https://fapi.binance.com/fapi/v1";
            _wsUrl = test ? "wss://fstream.binancefuture.com/ws" : "wss://fstream.binance.com";
        }


        #region ListenWebSocketAsync
        /// <summary>
        /// 初始化WebSocket，開始監聽User Data Stream
        /// </summary>
        public async Task ListenWebSocketAsync()
        {
            try
            {
                var listenKey = await CreateUserDataStreamAsync();

                _webSocket = new ClientWebSocket();
                _cts = new CancellationTokenSource();
                var wsUri = new Uri($"{_wsUrl}/{listenKey}");
                await _webSocket.ConnectAsync(wsUri, _cts.Token);
                Console.WriteLine("Binance WS Connected!");

                _ = Task.Run(ReceiveLoopAsync);
            }
            catch (Exception ex)
            {
                throw new Exception($"Binance WS Error: {ex.Message}");
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

        #endregion

        #region Create Order Process
        /// <summary>
        /// 下單流程
        /// </summary>
        /// <param name="request"></param>
        /// <param name="waitFinalStatus">是否等待監聽回傳交易資訊</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<OrderResponse> CreateOrderProcessAsync(OrderRequest request, bool waitFinalStatus = true)
        {
            var response = new OrderResponse() { Symbol = request.Symbol , OrderSideStatus = request.Side};

            await ChangePositionModeAsync(true);

            var changeLeverageResult = new BinanceChangeLeverageRequestDto(){
                Symbol = request.Symbol, 
                Laverage = request.Laverage ,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            };
            await ChangeLeverageAsync(changeLeverageResult);

            var amount = BinanceCalcuteQtyHelper.CalcuteQty(request);

            var binanceRequest = new BinanceCreateOrderRequestDto()
            {
                Symbol = request.Symbol,
                Side = request.Side,
                Quantity = amount,
                NewClientOrderId = "Binance_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            TaskCompletionSource<string>? tcs = null;
            if (waitFinalStatus)
            {
                tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                _orderTcsMap[binanceRequest.NewClientOrderId] = tcs;
            }

            var createResponse = await SendCreateOrderRequest(binanceRequest);

            if (!waitFinalStatus)
            {
                response.ClientOrderId = createResponse?.ClientOrderId ?? string.Empty;
                response.Price = Decimal.TryParse(createResponse?.Price, out var price) ? price : 0;
                response.Quantity = Decimal.TryParse(createResponse?.OrigQty, out var qty) ? qty : 0;
                return response;
            }
            else
            {
                try
                {
                    var finalStatus = await tcs!.Task;
                    Console.WriteLine($"訂單 {binanceRequest.NewClientOrderId} 狀態: {finalStatus}");
                    var listenInfo = _oderListenInfo[binanceRequest.NewClientOrderId];
                    response.ClientOrderId = listenInfo.ClientOrderId;
                    response.Price = Decimal.TryParse(listenInfo.LimitPrice, out var price) ? price : 0;
                    response.Quantity = Decimal.TryParse(listenInfo.Quantity, out var qty) ? qty : 0;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to get final status of order {binanceRequest.NewClientOrderId}: {ex.Message}");
                }
                finally
                {
                    _orderTcsMap.Remove(binanceRequest.NewClientOrderId);
                    _oderListenInfo.Remove(binanceRequest.NewClientOrderId);
                }
                return response;
            }
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
                if (request.Price == 0) throw new Exception("Price is required for LIMIT order");
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
        /// 切換持倉模式
        /// </summary>
        /// <param name="dualSidePosition">true 雙向持倉, false 單向持倉</param>
        /// <returns></returns>
        private async Task<BinanceChangePositionModeResponseDto?> ChangePositionModeAsync(bool dualSidePosition)
        {
            string endpoint = "/positionSide/dual";

            string url = $"{_baseUrl}{endpoint}";

            var parameters = new Dictionary<string, string>
            {
                { "dualSidePosition", dualSidePosition.ToString().ToLower() },
                { "timestamp",  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
            };

            string queryString = GetQueryString(parameters);
            string signature = GenerateSignature(queryString);

            parameters.Add("signature", signature);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<BinanceChangePositionModeResponseDto>(errorContent);
                if (error!.Code == (int)ErrorMsgStatus.NO_NEED_TO_CHANGE_POSITION_SIDE)
                {
                    return error; 
                }
                throw new Exception($"Error from Binance API: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BinanceChangePositionModeResponseDto>(content);
            return result;
        }

        /// <summary>
        /// 切換槓桿倍率
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<BinanceChangeLeverageResponseDto?> ChangeLeverageAsync(BinanceChangeLeverageRequestDto request)
        {
            string endpoint = "/leverage";

            string url = $"{_baseUrl}{endpoint}";

            var parameters = new Dictionary<string, string>
            {
                { "symbol", request.Symbol },
                { "leverage", request.Laverage.ToString() },
                { "timestamp",  request.Timestamp }
            };

            string queryString = GetQueryString(parameters);
            string signature = GenerateSignature(queryString);

            parameters.Add("signature", signature);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error from Binance API: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BinanceChangeLeverageResponseDto>(content);
            return result;
        }

        #endregion

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
