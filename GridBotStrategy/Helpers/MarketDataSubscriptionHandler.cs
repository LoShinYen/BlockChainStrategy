using BlockChainStrategy.Library.Models.Dto.Binance;
using GridBotStrategy.Observers;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace GridBotStrategy.Helpers
{
    internal class MarketDataSubscriptionHandler
    {
        private readonly List<IMarketDataObserver> _observers = new(); // 訂閱者列表
        private readonly ClientWebSocket _clientWebSocket;
        private readonly HeartbeatHandler _heartbeatManager;
        private readonly string _serverUri = "ws://localhost:5001/subscribe";
        private const int ReconnectDelayMs = 5000; // 重試間隔（毫秒）

        public MarketDataSubscriptionHandler()
        {
            _clientWebSocket = new ClientWebSocket();
            _heartbeatManager = new HeartbeatHandler();
        }

        public async Task ConnectAsync()
        {
            while (true)
            {
                try
                {
                    await _clientWebSocket.ConnectAsync(new Uri(_serverUri), CancellationToken.None);
                    LoggerHelper.LogInfo("成功連接到 WebSocket 統一接口");

                    // 啟動心跳機制
                    _heartbeatManager.Start(_clientWebSocket);

                    // 接收數據
                    await ReceiveMessagesAsync();
                }
                catch (Exception ex)
                {
                    await Task.Delay(ReconnectDelayMs);
                    LoggerHelper.LogError($"WebSocket 連接失敗：{ex.Message}，等待5秒");
                }
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var response = JsonConvert.DeserializeObject<BinanceMarketPriceDataDto>(message);
                        NotifyObservers(response!);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        LoggerHelper.LogInfo("伺服器關閉了 WebSocket 連接");
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError($"接收數據時出錯：{ex.Message}");
                    break;
                }
            }
        }

        internal void Subscribe(IMarketDataObserver observer)
        {
            _observers.Add(observer);
        }

        internal void Unsubscribe(IMarketDataObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyObservers(BinanceMarketPriceDataDto message)
        {
            foreach (var observer in _observers)
            {
                observer.OnMarketDataReceived(message);
            }
        }
    }
}
