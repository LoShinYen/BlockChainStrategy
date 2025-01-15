using BlockChainStrategy.Library.Models;

namespace GridBotStrategy.Helpers.Interface
{
    internal interface IMarketDataHandler
    {
        void UpdateMarketPrice(BinanceMarketPriceData message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
