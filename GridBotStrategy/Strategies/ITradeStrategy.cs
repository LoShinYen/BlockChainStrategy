namespace GridBotStrategy.Strategies
{
    public interface ITradeStrategy
    {
        Task ExecuteTradeAsync(TradeRobotInfo robot, decimal currentMarketPrice);
    }
}
