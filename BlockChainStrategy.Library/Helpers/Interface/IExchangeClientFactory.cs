namespace BlockChainStrategy.Library.Helpers.Interface
{
    public interface IExchangeClientFactory
    {
        IExchangeClient GetExchangeClient(ExchangeConfig config);
    }
}
