using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace UnifiedWsGateway.Services
{
    internal class BinanceMarketPriceService
    {
        internal event Action<string>? OnMarketDataReceived;

        private List<string> _marketSymbols = new List<string>();
        private string _baseStreamUrl = "wss://fstream.binance.com/stream?streams=";

        public BinanceMarketPriceService(List<string> marketSymbols)
        {
            _marketSymbols = marketSymbols;
        }

        internal async Task StartWebSocketService()
        {
            var tasks = new List<Task>();

            string streamUrl = _baseStreamUrl + string.Join("/", _marketSymbols);

            tasks.Add(Task.Run(async () => await ConnectWebSocketAsync(streamUrl)));

            await Task.WhenAll(tasks);
        }

        private async Task ConnectWebSocketAsync(string uri)
        {
            Uri serverUri = new Uri(uri);
            int retryDelay = 1000;

            while (true)
            {
                using (ClientWebSocket clientWebSocket = new ClientWebSocket())
                {
                    try
                    {
                        await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

                        await SentSubMessageAsync(clientWebSocket);

                        await HandleWebsocketMessageAsync(clientWebSocket);
                    }
                    catch (WebSocketException ex)
                    {
                        LoggerHelper.LogError($"WebSocket 發生錯誤: {ex.Message}, 正在嘗試重新連接...");
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.LogError($"未知錯誤: {ex.Message}");
                    }
                    finally
                    {
                        LoggerHelper.LogInfo($"WebSocket重新連線，請等待{retryDelay /1000}秒");
                        await Task.Delay(retryDelay);
                        retryDelay = Math.Min(retryDelay * 2, 30000);
                    }
                }
            }
        }

        private async Task HandleWebsocketMessageAsync(ClientWebSocket clientWebSocket)
        {
            var buffer = new byte[8192];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (message.Contains(@"""result"":null")) continue;
                    RaiseMarketPriceEvent(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"WebSocket closed by server: {clientWebSocket.CloseStatus}, Reason: {clientWebSocket.CloseStatusDescription}");
                    break;
                }
            }
        }

        private async Task SentSubMessageAsync(ClientWebSocket clientWebSocket)
        {
            var subscribeMessage = new
            {
                method = "SUBSCRIBE",
                @params = _marketSymbols,
                id = 1
            };

            var subscribeJson = System.Text.Json.JsonSerializer.Serialize(subscribeMessage);
            var subscribeBytes = Encoding.UTF8.GetBytes(subscribeJson);

            await clientWebSocket.SendAsync(
                new ArraySegment<byte>(subscribeBytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        private void RaiseMarketPriceEvent(string message)
        {
            try
            {
                var marketPriceData = JsonConvert.DeserializeObject<BinanceMarketPriceMessage>(message);
                var pubStr = JsonConvert.SerializeObject(marketPriceData!.Data);
                OnMarketDataReceived?.Invoke(pubStr);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError($"處理行情數據時發生錯誤: {ex.Message}");
            }
        }
    }
}
