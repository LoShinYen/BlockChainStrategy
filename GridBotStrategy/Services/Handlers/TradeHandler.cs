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
                p.TargetPrice <= maxPrice );

            if (position == null)
            {
                return false;
            }
            else
            {
                // 符合上次下單價格，且已啟動，且為最後一個目標價格
                if (position.TargetPrice == robot.LastTargetPositionPrice && position.IsLastTarget == true && position.IsActivated == true)
                {
                    return false;
                }
                // 符合上次下單、未被啟動、倉為尚平 
                if (position.IsActivated == false && position.IsLastTarget == true && robot.CurrentPositionCount !=0 )
                { 
                    return false;
                }
            }

            robot.TargetPositionIndex = position.TargetIndex;
            robot.LastTargetPositionPrice = position.TargetPrice;

            return true;
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
                    robot.CurrentPositionCount += adjustmentCount;
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
                        if (robot.CurrentPositionCount == 0)
                        {
                            robot.AvgHoldingPrice = 0;
                            robot.Postions.ForEach(p => p.IsActivated = false);
                        }
                        else
                        { 
                            robot.AvgHoldingPrice = (orQty * robot.AvgHoldingPrice - order.Quantity * order.Price) / (orQty - order.Quantity);
                            robot.Postions[robot.TargetPositionIndex].IsActivated = false;
                        }
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

                //Rest Robot Postions IsLastTarget
                robot.Postions.ForEach(p => p.IsLastTarget = false);
                robot.Postions[robot.TargetPositionIndex].IsLastTarget = true;

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
