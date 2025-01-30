namespace GridBotStrategy.Services.Strategies
{
    public interface ITradeStrategy
    {
        Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot);

        void UpdatePositionInfo(OrderResponse order, TradeRobotInfo robot);
    }
}
