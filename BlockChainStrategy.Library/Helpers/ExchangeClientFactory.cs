using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Exchange;
using BlockChainStrategy.Library.Helpers.Interface;

namespace BlockChainStrategy.Library.Helpers
{
    public class ExchangeClientFactory : IExchangeClientFactory
    {
        public IExchangeClient GetExchangeClient(ExchangeConfig config)
        {
            return config.ExchangeType switch
            {
                ExchangeType.Binance => new BinanceClient(config.ApiKey ?? string.Empty, config.ApiSecret ?? string.Empty, config.Test),
                _ => throw new ArgumentException("Invalid exchange type")
            };
        }
    }
}
