using GridBotStrategy.Helpers.TradeStrategyFactory;

namespace GridBotStrategy.Services.Handlers
{
    internal class TradeHandler : ITradeHandler
    {
        private readonly IMarketDataHelper _marketDataHandler;

        public TradeHandler(IMarketDataHelper marketDataHandler)
        {
            _marketDataHandler = marketDataHandler;
        }

        public async Task HandleTradeAsync(TradeRobotInfo robot)
        {
            if (!_marketDataHandler.TryGetCurrentPrice(robot.Symbol, out var currentMarketPrice))
                return;

            robot.CurrentPrice = currentMarketPrice;

            var minPrice = Math.Min(robot.LastPrice, robot.CurrentPrice);
            var maxPrice = Math.Max(robot.LastPrice, robot.CurrentPrice);

            if (CheckTargetPrice(robot, minPrice, maxPrice) && SelectTargetIndex(robot, minPrice, maxPrice))
            {
                var strategy = StrategyFactory.GetStrategy(robot.PositionSideEnum);
                await strategy.ExecuteTradeAsync(robot);
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
