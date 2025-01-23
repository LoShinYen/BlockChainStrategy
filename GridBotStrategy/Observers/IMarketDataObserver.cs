using BlockChainStrategy.Library.Models.Dto.Binance;

namespace GridBotStrategy.Observers
{
    internal interface IMarketDataObserver
    {
        void OnMarketDataReceived(BinanceMarketPriceDataDto message);
    }
}
