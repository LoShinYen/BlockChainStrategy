using GridBotStrategy.Services.Strategies;

namespace GridBotStrategy.Helpers.TradeStrategyFactory
{
    internal class StrategyFactory
    {
        public static ITradeStrategy GetStrategy(GridTradeRobotPositionSide positionSide)
        {
            return positionSide switch
            {
                GridTradeRobotPositionSide.Long => new LongTradeStrategy(),
                GridTradeRobotPositionSide.Short => new ShortTradeStrategy(),
                GridTradeRobotPositionSide.All => new NeutralTradeStrategy(),
                _ => throw new ArgumentException("Invalid position side")
            };
        }
    }
}
