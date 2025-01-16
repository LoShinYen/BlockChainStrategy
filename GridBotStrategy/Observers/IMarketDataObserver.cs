using BlockChainStrategy.Library.Models.Dto;

namespace GridBotStrategy.Observers
{
    internal interface IMarketDataObserver
    {
        void OnMarketDataReceived(BinanceMarketPriceDataDto message);
    }
}
