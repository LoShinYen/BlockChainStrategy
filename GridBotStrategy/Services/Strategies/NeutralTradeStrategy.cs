namespace GridBotStrategy.Services.Strategies
{
    public class NeutralTradeStrategy : ITradeStrategy
    {
        private readonly TradeOperationService _tradeOperationService;

        public NeutralTradeStrategy(TradeOperationService tradeOperationService)
        {
            _tradeOperationService = tradeOperationService;
        }

        public Task ExecuteTradeAsync(TradeRobotInfo robot)
        {
            throw new NotImplementedException();
        }
    }
}