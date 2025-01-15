namespace GridBotStrategy.Services.Strategies
{
    public class ShortTradeStrategy : ITradeStrategy
    {
        private readonly TradeOperationService _tradeOperationService;

        public ShortTradeStrategy(TradeOperationService tradeOperationService)
        {
            _tradeOperationService = tradeOperationService;
        }

        public Task ExecuteTradeAsync(TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }
    }

}