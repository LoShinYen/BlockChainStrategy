using BlockChainStrategy.Library.Models;
using GridBotStrategy.Observers;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        public void OnMarketDataReceived(BinanceMarketPriceData message)
        {
            Console.WriteLine($"接收到市場數據：{message.Symbol} - {message.MarkPrice}");
        }

        public async Task ExcuteTradeAsync()
        {
            while(true)
            {
            
            }
        }

    }
}
