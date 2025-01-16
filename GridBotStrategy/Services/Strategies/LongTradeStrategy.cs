namespace GridBotStrategy.Services.Strategies
{
    public class LongTradeStrategy : BaseStratgyService, ITradeStrategy
    {

        public async Task ExecuteTradeAsync(TradeRobotInfo robot)
        {
            if (CheckIsOpen(robot.CurrentPositionCount))
            {
                await OpenPositionAsync();
            }
            else
            {
                if (CheckPriceIsRaise(robot.LastPrice, robot.CurrentPrice))
                {
                    await RaisePositionAsync();
                }
                else
                {
                    await ClosePositionAsync();
                }
            }
        }
    }
}
