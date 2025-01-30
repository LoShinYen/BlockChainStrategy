using BlockChainStrategy.Library.Helpers.Interface;
using GridBotStrategy.Services.Strategies;

namespace GridBotStrategy.Helpers.TradeStrategyFactory
{
    public class StrategyFactory : IStrategyFactory
    {
        private readonly IExchangeClientFactory _exchangeClientFactory;

        public StrategyFactory(IExchangeClientFactory exchangeClientFactory)
        {
            _exchangeClientFactory = exchangeClientFactory;
        }

        public ITradeStrategy GetStrategy(GridTradeRobotPositionSide positionSide)
        {
            return positionSide switch
            {
                GridTradeRobotPositionSide.Long => new LongTradeStrategy(_exchangeClientFactory),
                GridTradeRobotPositionSide.Short => new ShortTradeStrategy(_exchangeClientFactory),
                GridTradeRobotPositionSide.All => new NeutralTradeStrategy(_exchangeClientFactory),
                _ => throw new ArgumentException("Invalid position side")
            };
        }
    }
}
