using BlockChainStrategy.Library.Models.Dto;
using BlockChainStrategy.Library.Models.Dto.Utility;

namespace BlockChainStrategy.Library.Exchange.Interface
{
    public interface IExchangeClient
    {
        Task ListenWebSocketAsync();

        Task<OrderResponse?> CreateOrderAsync(OrderRequest request, bool waitFinalStatus = true);
    }
}
