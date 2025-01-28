namespace BlockChainStrategy.Library.Exchange.Interface
{
    public interface IExchangeClient
    {
        Task ListenWebSocketAsync();

        Task<OrderResponse?> CreateOrderProcessAsync(OrderRequest request, bool waitFinalStatus = true);
    }
}
