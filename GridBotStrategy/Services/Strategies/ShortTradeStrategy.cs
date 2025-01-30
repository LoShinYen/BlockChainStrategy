namespace GridBotStrategy.Services.Strategies
{
    public class ShortTradeStrategy : BaseStratgyService, ITradeStrategy
    {

        public Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }

        public void UpdatePositionInfo(OrderResponse order, TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }

}