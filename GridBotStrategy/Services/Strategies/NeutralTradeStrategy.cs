namespace GridBotStrategy.Services.Strategies
{
    public class NeutralTradeStrategy : BaseStratgyService, ITradeStrategy
    {
        public Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }
    }
}