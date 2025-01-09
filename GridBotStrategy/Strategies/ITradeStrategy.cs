namespace GridBotStrategy.Strategies
{
    internal interface ITradeStrategy
    {
        Task ExecuteTradeAsync(GridTradeRobot robot, decimal currentMarketPrice);
    }
}
