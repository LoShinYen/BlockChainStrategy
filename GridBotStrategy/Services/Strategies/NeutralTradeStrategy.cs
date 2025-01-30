using BlockChainStrategy.Library.Helpers.Interface;

namespace GridBotStrategy.Services.Strategies
{
    public class NeutralTradeStrategy : BaseStratgyService, ITradeStrategy
    {
        public NeutralTradeStrategy(IExchangeClientFactory exchangeClientFactory) : base(exchangeClientFactory)
        {
        }

        public Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }

        public void UpdatePositionInfo(OrderResponse order, TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }
    }
}