using GridBotStrategy.Observers;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        public void OnMarketDataReceived(string message)
        {
            throw new NotImplementedException();
        }

        public async Task ExcuteTradeAsync()
        {
            
        }

    }
}
