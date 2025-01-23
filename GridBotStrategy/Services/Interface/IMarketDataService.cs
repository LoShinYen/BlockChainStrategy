using BlockChainStrategy.Library.Models.Dto.Binance;

namespace GridBotStrategy.Services.Interface
{
    internal interface IMarketDataService
    {
        void UpdateMarketPrice(BinanceMarketPriceDataDto message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
