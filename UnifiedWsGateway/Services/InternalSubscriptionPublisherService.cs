using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace UnifiedWsGateway.Services
{
    internal class InternalSubscriptionPublisherService
    {
        // 使用 static 關鍵字來確保所有執行緒共享這些集合
        private static ConcurrentDictionary<WebSocket, SemaphoreSlim> _clientLocks = new ConcurrentDictionary<WebSocket, SemaphoreSlim>();
        private static ConcurrentBag<WebSocket> _subscribedClients = new ConcurrentBag<WebSocket>();
        private static ConcurrentDictionary<WebSocket, bool> _connectedClients = new ConcurrentDictionary<WebSocket, bool>();
        private static ConcurrentDictionary<WebSocket, string> _clientInfo = new ConcurrentDictionary<WebSocket, string>();


        internal void SubscribeMarketData(BinanceMarketPriceService binanceService)
        {
            binanceService.OnMarketDataReceived += async (data) =>
            {
                await PushMarketDataToClients(data);
            };
        }

        internal async Task HandleClientWebSocketAsync(WebSocket webSocket, IPEndPoint remoteEndPoint)
        {
            if (!_connectedClients.TryAdd(webSocket, true)) return;

            var clientIp = remoteEndPoint.Address.ToString();
            var clientPort = remoteEndPoint.Port;

            _clientInfo.TryAdd(webSocket, $"{clientIp}:{clientPort}");
            _subscribedClients.Add(webSocket);
            _clientLocks.TryAdd(webSocket, new SemaphoreSlim(1, 1));

            LoggerHelper.LogAndShowInfo($"Client connected: {clientIp}:{clientPort}");
            try
            {
                // 保持 WebSocket 連接
                while (webSocket.State == WebSocketState.Open)
                {
                    await Task.Delay(1000);
                }
            }
            finally
            {
                _clientLocks.TryRemove(webSocket, out _); // 移除對應的鎖
                _connectedClients.TryRemove(webSocket, out _); // 從連線集合中移除
                _subscribedClients = new ConcurrentBag<WebSocket>(_subscribedClients.Where(ws => ws != webSocket)); // 重建 _subscribedClients 排除斷開的客戶端
                _clientInfo.TryRemove(webSocket, out _); // 移除 IP 和 Port 映射
            }
        }

        private async Task PushMarketDataToClients(string data)
        {
            var messageBytes = Encoding.UTF8.GetBytes(data);
            List<WebSocket> disconnectedClients = new List<WebSocket>(); // 用於收集已中斷的客戶端

            foreach (var client in _subscribedClients)
            {
                var semaphore = _clientLocks[client];
                await semaphore.WaitAsync(); // 獲取鎖，確保每個連線的發送操作不重疊
                try
                {
                    // 檢查 WebSocket 連線是否仍然開啟並直接發送數據
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (WebSocketException)
                        {
                            // 發送失敗則加入到中斷列表
                            disconnectedClients.Add(client);
                        }
                    }
                    else
                    {
                        disconnectedClients.Add(client);
                    }
                }
                finally
                {
                    semaphore.Release(); // 釋放鎖
                }
            }

            // 批量移除已中斷的客戶端
            foreach (var client in disconnectedClients)
            {
                _subscribedClients = new ConcurrentBag<WebSocket>(_subscribedClients.Where(ws => ws != client));
                _clientLocks.TryRemove(client, out _); // 移除該客戶端的鎖
                _connectedClients.TryRemove(client, out _); // 從連線集合中移除
                LoggerHelper.LogAndShowInfo($"Disconnected client removed: {_clientInfo.GetValueOrDefault(client, "Unknown")}");
                _clientInfo.TryRemove(client, out _);
            }
        }
    }
}
