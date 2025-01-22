using BlockChainStrategy.Library.Models.Dto;

namespace GridBotStrategy.Services.Interface
{
    internal interface IMarketDataService
    {
        void UpdateMarketPrice(BinanceMarketPriceDataDto message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
