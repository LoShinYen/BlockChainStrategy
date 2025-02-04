using BlockChainStrategy.Library.Models.Dto.Binance;

namespace GridBotStrategy.Services.Interface
{
    public interface IMarketDataService
    {
        void UpdateMarketPrice(BinanceMarketPriceDataDto message);

        bool TryGetCurrentPrice(string symbol, out decimal currentPrice);
    }
}
