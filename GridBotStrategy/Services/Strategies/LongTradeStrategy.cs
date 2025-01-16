namespace GridBotStrategy.Services.Strategies
{
    public class LongTradeStrategy : ITradeStrategy
    {
        private readonly TradeOperationService _tradeOperationService;

        public LongTradeStrategy(TradeOperationService tradeOperationService)
        {
            _tradeOperationService = tradeOperationService;
        }

        public async Task ExecuteTradeAsync(TradeRobotInfo robot)
        {
            if (_tradeOperationService.CheckIsOpen(robot.CurrentPositionCount))
            {
                await _tradeOperationService.OpenPositionAsync();
            }
            else
            {
                if (_tradeOperationService.CheckPriceIsRaise(robot.LastPrice, robot.CurrentPrice))
                {
                    await _tradeOperationService.RaisePositionAsync();
                }
                else
                {
                    await _tradeOperationService.ClosePositionAsync();
                }
            }
        }
    }
}
