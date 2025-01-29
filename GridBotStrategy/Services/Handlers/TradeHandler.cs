using BlockChainStrategy.Library.Enums;
using GridBotStrategy.Helpers.TradeStrategyFactory;

namespace GridBotStrategy.Services.Handlers
{
    internal class TradeHandler : ITradeHandler
    {
        private readonly IMarketDataService _marketDataService;
        private readonly CryptoPlatformDbContext _dbContext;
        private readonly IGridTradeRobotRepository _gridRobotRepository;
        private readonly IGridTradeRobotDetailRepository _gridTradeRobotDetailRepository;
        private readonly IGridTradeRobotOrderRepository _gridTradeRobotOrderRepository;
        private readonly IGridTradeRobotOrderHostoryRepository _gridTradeRobotOrderHostoryRepository;

        public TradeHandler(
            IMarketDataService marketDataService, 
            CryptoPlatformDbContext dbContext,
            IGridTradeRobotRepository robotRepository,
            IGridTradeRobotDetailRepository gridTradeRobotDetailRepository,
            IGridTradeRobotOrderRepository gridTradeRobotOrderRepository,
            IGridTradeRobotOrderHostoryRepository gridTradeRobotOrderHostoryRepository
        )
        {
            _marketDataService = marketDataService;
            _dbContext = dbContext;
            _gridRobotRepository = robotRepository;
            _gridTradeRobotDetailRepository = gridTradeRobotDetailRepository;
            _gridTradeRobotOrderRepository = gridTradeRobotOrderRepository;
            _gridTradeRobotOrderHostoryRepository = gridTradeRobotOrderHostoryRepository;
        }

        public async Task HandleTradeAsync(TradeRobotInfo robot)
        {
            if (!_marketDataService.TryGetCurrentPrice(robot.Symbol, out var currentMarketPrice))
                return;

            robot.CurrentPrice = currentMarketPrice;

            var minPrice = Math.Min(robot.LastPrice, robot.CurrentPrice);
            var maxPrice = Math.Max(robot.LastPrice, robot.CurrentPrice);

            if (CheckTargetPrice(robot, minPrice, maxPrice) && SelectTargetIndex(robot, minPrice, maxPrice))
            {
                var strategy = StrategyFactory.GetStrategy(robot.PositionSideEnum);
                var response = await strategy.ExecuteTradeAsync(robot);
                await HandleOrderInfoProcessAsync(response, robot);
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
    
        
        public async Task TestCreateOrderAsync(TradeRobotInfo robot)
        {
            if (robot.Symbol == "BTCUSDT")
            {
                var currentMarketPrice = 101000;
                robot.LastPrice = 100900;

                robot.CurrentPrice = currentMarketPrice;

                var minPrice = Math.Min(robot.LastPrice, robot.CurrentPrice);
                var maxPrice = Math.Max(robot.LastPrice, robot.CurrentPrice);

                if (CheckTargetPrice(robot, minPrice, maxPrice) && SelectTargetIndex(robot, minPrice, maxPrice))
                {
                    var strategy = StrategyFactory.GetStrategy(robot.PositionSideEnum);
                    var response = await strategy.ExecuteTradeAsync(robot);
                    await HandleOrderInfoProcessAsync(response,robot);
                }
                robot.LastPrice = currentMarketPrice;
            }
        }

        private async Task HandleOrderInfoProcessAsync(OrderResponse order, TradeRobotInfo robot)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var detailInfo = await _gridTradeRobotDetailRepository.GetDetailByRobotIdAsync(robot.RobotId);
                if(detailInfo == null) throw new Exception("找不到對應的Detail資料");

                var runningOrder = await _gridTradeRobotOrderRepository.GetRunningOrderByRobotIdAsync(robot.RobotId);

                int adjustmentCount = order.OrderSideStatus == OrderSideStatus.BUY ? 1 :-1;

                //如果有正在執行的訂單
                if (runningOrder != null)
                {
                    runningOrder.UpdatedAt = DateTime.UtcNow;
                    runningOrder.TradeAmount += order.Quantity;

                    //網格歸0，關閉訂單
                    robot.CurrentPositionCount -= adjustmentCount;
                    if (robot.CurrentPositionCount == 0)
                    {
                        runningOrder.Status = "Finish";
                        robot.StatusEnum = GridTradeRobotStatus.Open;
                    }
                    _gridTradeRobotOrderRepository.UpdateRobotOrder(runningOrder);
                }
                //如果沒有正在執行的訂單
                else
                {
                    runningOrder = new GridTradeRobotOrder()
                    {
                        GridTradeRobotId = robot.RobotId,
                        Status = "Running",
                        TradeAmount = order.Quantity,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _gridTradeRobotOrderRepository.CreateRobotOrderAsync(runningOrder);
                    robot.CurrentPositionCount += adjustmentCount;
                    robot.StatusEnum = GridTradeRobotStatus.Running;
                }

                #region OrderHistory
                var history = new GridTradeRobotOrderHistory()
                {
                    GridTradeRobotOrderId = runningOrder.GridTradeRobotOrderId,
                    Price = order.Price,
                    TradeAction = order.OrderSideStatus.ToString(),
                    TradeAmount = order.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                await _gridTradeRobotOrderHostoryRepository.CreateRobotOrderHistoryAsync(history);
                #endregion

                #region Update Detail
                if (robot.PositionSideEnum == GridTradeRobotPositionSide.Long)
                {
                    // IsActivated = true 代表啟動
                    if (order.OrderSideStatus == OrderSideStatus.BUY)
                    {
                        var orQty = robot.HoldingQty;
                        robot.AvgHoldingPrice = (orQty * robot.AvgHoldingPrice + order.Quantity * order.Price) / (orQty + order.Quantity);

                        robot.Postions[robot.TargetPositionIndex].IsActivated = true;
                        robot.HoldingQty += order.Quantity;
                    }
                    // IsActivated = false 
                    else
                    { 
                        var orQty = robot.HoldingQty;
                        robot.AvgHoldingPrice = (orQty * robot.AvgHoldingPrice - order.Quantity * order.Price) / (orQty - order.Quantity);
                        robot.Postions[robot.TargetPositionIndex].IsActivated = false;
                        robot.HoldingQty -= order.Quantity;
                    }
                }
                else if (robot.PositionSideEnum == GridTradeRobotPositionSide.Short)
                {
                    throw new Exception("尚未支援做空機器人");
                }
                else
                {
                    throw new Exception("尚未支援中性機器人");
                }

                detailInfo.AvgPrice = robot.AvgHoldingPrice;
                detailInfo.HoldingAmount = robot.HoldingQty;
                detailInfo.Postions = robot.Postions;
                detailInfo.CurrentPositionCount = robot.CurrentPositionCount;
                _gridTradeRobotDetailRepository.UpdateRobotDetail(detailInfo);

                #endregion

                _gridRobotRepository.UpdateRobotByOrderProccssed(robot);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                LoggerHelper.LogError($"發生錯誤：{e.Message}");
            }
        }
    }
}
