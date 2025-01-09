namespace GridBotStrategy.Strategies
{
    internal class TradeStrategyFactory
    {
        public static ITradeStrategy GetStrategy(GridTradeRobotPositionSide positionSide)
        {
            return positionSide switch
            {
                GridTradeRobotPositionSide.Long => new LongTradeStrategy(),
                GridTradeRobotPositionSide.Short => new ShortTradeStrategy(),
                GridTradeRobotPositionSide.All => new AllTradeStrategy(),
                _ => throw new ArgumentException("Invalid position side")
            };
        }
    }
}
