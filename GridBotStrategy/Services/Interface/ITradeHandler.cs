namespace GridBotStrategy.Services.Interface
{
    internal interface ITradeHandler
    {
        Task HandleTradeAsync(TradeRobotInfo robot);

        Task TestCreateOrderAsync(TradeRobotInfo robot);

    }
}
