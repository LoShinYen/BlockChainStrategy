using GridBotStrategy.Strategies;

namespace GridBotStrategy.Services
{
    internal class TradeHandler : ITradeHandler
    {
        private readonly IMarketDataHandler _marketDataHandler;

        public TradeHandler(IMarketDataHandler marketDataHandler)
        {
            _marketDataHandler = marketDataHandler;
        }

        public async Task HandleTradeAsync(TradeRobotInfo robot)
        {
            if (!_marketDataHandler.TryGetCurrentPrice(robot.Symbol, out var currentMarketPrice))
                return;

            var minPrice = Math.Min(robot.LastPrice, currentMarketPrice);
            var maxPrice = Math.Max(robot.LastPrice, currentMarketPrice);

            if (CheckTargetPrice(robot, minPrice, maxPrice) && SelectTargetIndex(robot, minPrice, maxPrice))
            {
                var strategy = TradeStrategyFactory.GetStrategy(robot.PositionSideEnum);
                await strategy.ExecuteTradeAsync(robot, currentMarketPrice);
            }

            robot.LastPrice = currentMarketPrice;
        }

        private bool CheckTargetPrice(TradeRobotInfo robot, decimal minPrice, decimal maxPrice)
        {
            return robot.Postions.Any(p => p.TargetPrice >= minPrice && p.TargetPrice <= maxPrice);
        }

        private bool SelectTargetIndex(TradeRobotInfo robot, decimal minPrice, decimal maxPrice)
        {
            var position = robot.Postions.FirstOrDefault(p =>
                p.TargetPrice >= minPrice &&
                p.TargetPrice <= maxPrice &&
                p.TargetPrice != robot.LastTargetPositionPrice);

            if (position == null)
                return false;

            robot.TargetPositionIndex = position.TargetIndex;
            robot.LastTargetPositionPrice = position.TargetPrice;
            return true;
        }
    }

}
