using BlockChainStrategy.Library.Models.Dto;

namespace GridBotStrategy.Helpers
{
    internal class MarketDataHelper : IMarketDataHelper
    {
        private Dictionary<string, decimal> _symbolMarkPrice = new();

        public void UpdateMarketPrice(BinanceMarketPriceDataDto message)
        {
            if (decimal.TryParse(message.MarkPrice, out var price))
            {
                if (_symbolMarkPrice.ContainsKey(message.Symbol))
                {
                    _symbolMarkPrice[message.Symbol] = price;
                }
                else
                {
                    _symbolMarkPrice.Add(message.Symbol, price);
                }
            }
            else
            {
                LoggerHelper.LogError($"無法解析價格：{message.MarkPrice}");
            }
        }

        public bool TryGetCurrentPrice(string symbol, out decimal currentPrice)
        {
            return _symbolMarkPrice.TryGetValue(symbol, out currentPrice);
        }
    }
}
