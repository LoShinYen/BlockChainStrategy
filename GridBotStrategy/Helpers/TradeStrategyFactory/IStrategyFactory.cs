using GridBotStrategy.Services.Strategies;

namespace GridBotStrategy.Helpers.TradeStrategyFactory
{
    public interface IStrategyFactory
    {
        ITradeStrategy GetStrategy(GridTradeRobotPositionSide positionSide);
    }
}
