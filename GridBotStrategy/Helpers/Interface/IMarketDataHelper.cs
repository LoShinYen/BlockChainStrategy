using BlockChainStrategy.Library.Models.Dto;

namespace GridBotStrategy.Helpers.Interface
{
    internal interface IMarketDataHelper
    {
        void UpdateMarketPrice(BinanceMarketPriceDataDto message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
