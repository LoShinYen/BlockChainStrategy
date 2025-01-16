using BlockChainStrategy.Library.Models;

namespace GridBotStrategy.Helpers.Interface
{
    internal interface IMarketDataHelper
    {
        void UpdateMarketPrice(BinanceMarketPriceData message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
