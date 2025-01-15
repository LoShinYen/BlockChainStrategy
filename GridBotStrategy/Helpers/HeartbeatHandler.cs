using System.Net.WebSockets;
using System.Text;

namespace GridBotStrategy.Helpers
{
    internal class HeartbeatHandler
    {

        private const int HeartbeatIntervalMs = 15 * 60 * 1000; // 心跳間隔 15 分鐘
        private CancellationTokenSource? _cancellationTokenSource;

        public void Start(ClientWebSocket clientWebSocket)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => SendHeartbeatAsync(clientWebSocket, _cancellationTokenSource.Token));
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null)
            { 
                _cancellationTokenSource.Cancel();
            }
        }

        private async Task SendHeartbeatAsync(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (clientWebSocket.State == WebSocketState.Open)
                    {
                        var heartbeatMessage = Encoding.UTF8.GetBytes("ping");
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(heartbeatMessage), WebSocketMessageType.Text, true, cancellationToken);
                        LoggerHelper.LogInfo("已發送心跳包");
                    }
                    await Task.Delay(HeartbeatIntervalMs, cancellationToken);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError($"發送心跳包時出錯：{ex.Message}");
                    break;
                }
            }
        }
    }
}

