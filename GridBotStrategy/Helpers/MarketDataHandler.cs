using BlockChainStrategy.Library.Models;
using GridBotStrategy.Helpers.Interface;

namespace GridBotStrategy.Helpers
{
    internal class MarketDataHandler : IMarketDataHandler
    {
        private readonly Dictionary<string, decimal> _symbolMarkPrice = new();

        public void UpdateMarketPrice(BinanceMarketPriceData message)
        {
            decimal.TryParse(message.MarkPrice, out var price);
            _symbolMarkPrice[message.Symbol] = price;
        }

        public bool TryGetCurrentPrice(string symbol, out decimal currentPrice)
        {
            return _symbolMarkPrice.TryGetValue(symbol, out currentPrice);
        }
    }
}
