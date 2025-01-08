using BlockChainStrategy.Library.Models;

namespace GridBotStrategy.Observers
{
    internal interface IMarketDataObserver
    {
        void OnMarketDataReceived(BinanceMarketPriceData message);
    }
}
