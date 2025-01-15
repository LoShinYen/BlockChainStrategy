namespace GridBotStrategy.Services.Strategies
{
    public interface ITradeStrategy
    {
        Task ExecuteTradeAsync(TradeRobotInfo robot);
    }
}
