using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Exchange;

namespace BlockChainStrategy.Library.Helpers
{
    public class ExchangeFactory
    {
        public static IExchangeClient GetExchangeClient(ExchangeConfig config)
        {
            return config.ExchangeType switch
            {
                ExchangeType.Binance => new BinanceClient(config.ApiKey ?? string.Empty, config.ApiSecret ?? string.Empty, config.Test),
                _ => throw new ArgumentException("Invalid exchange type")
            };
        }
    }
}
