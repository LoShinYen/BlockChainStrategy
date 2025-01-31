using BlockChainStrategy.Library.Enums;
using GridBotStrategy.Helpers.TradeStrategyFactory;
using GridBotStrategy.Services.Strategies;

namespace GridBotStrategy.Services.Handlers
{
    internal class TradeHandler : ITradeHandler
    {
        private readonly IMarketDataService _marketDataService;
        private readonly IStrategyFactory _strategyFactory;
        private readonly CryptoPlatformDbContext _dbContext;
        private readonly IGridTradeRobotRepository _gridRobotRepository;
        private readonly IGridTradeRobotDetailRepository _gridTradeRobotDetailRepository;
        private readonly IGridTradeRobotOrderRepository _gridTradeRobotOrderRepository;
        private readonly IGridTradeRobotOrderHostoryRepository _gridTradeRobotOrderHostoryRepository;

        public TradeHandler(
            IMarketDataService marketDataService, 
            IStrategyFactory strategyFactory,
            CryptoPlatformDbContext dbContext,
            IGridTradeRobotRepository robotRepository,
            IGridTradeRobotDetailRepository gridTradeRobotDetailRepository,
            IGridTradeRobotOrderRepository gridTradeRobotOrderRepository,
            IGridTradeRobotOrderHostoryRepository gridTradeRobotOrderHostoryRepository
        )
        {
            _marketDataService = marketDataService;
            _strategyFactory = strategyFactory;
            _dbContext = dbContext;
            _gridRobotRepository = robotRepository;
            _gridTradeRobotDetailRepository = gridTradeRobotDetailRepository;
            _gridTradeRobotOrderRepository = gridTradeRobotOrderRepository;
            _gridTradeRobotOrderHostoryRepository = gridTradeRobotOrderHostoryRepository;
        }

        public async Task HandleTradeAsync(TradeRobotInfo robot)
        {
            if (!_marketDataService.TryGetCurrentPrice(robot.Symbol, out var currentMarketPrice)) return;

            robot.CurrentPrice = currentMarketPrice;

            // 預防程式剛啟動時，上次價格為0
            if (robot.LastPrice == 0 && currentMarketPrice != 0)
            { 
                robot.LastPrice = currentMarketPrice;
            }

            var minPrice = Math.Min(robot.LastPrice, robot.CurrentPrice);
            var maxPrice = Math.Max(robot.LastPrice, robot.CurrentPrice);

            if (TrySelectPosition(robot, minPrice, maxPrice))
            {
                var strategy = _strategyFactory.GetStrategy(robot.PositionSideEnum);
                var response = await strategy.ExecuteTradeAsync(robot);
                await HandleOrderInfoProcessAsync(response, robot, strategy);
            }

            robot.LastPrice = currentMarketPrice;
        }

        private bool TrySelectPosition(TradeRobotInfo robot, decimal minPrice, decimal maxPrice)
        {
            var position = robot.Postions.FirstOrDefault(p => p.TargetPrice >= minPrice && p.TargetPrice <= maxPrice );
                
            if (position == null) return false;

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

        private async Task HandleOrderInfoProcessAsync(OrderResponse order, TradeRobotInfo robot,ITradeStrategy strategy)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var detailInfo = await _gridTradeRobotDetailRepository.GetDetailByRobotIdAsync(robot.RobotId);
                if (detailInfo == null) throw new Exception("找不到對應的Detail資料");

                //Setp 1: Update Robot Info
                var runningOrder = await UpdateRunningOrderAsync(order, robot);

                //Setp 2: Create Order History
                await CreateOrderHistoyAsync(order, runningOrder.GridTradeRobotOrderId);

                //Setp 3: Update Running Order Info
                strategy.UpdatePositionInfo(order, robot);

                //Setp 4: Update Detail Info
                UpdateOrderDetail(robot, detailInfo);

                _gridRobotRepository.UpdateRobotByOrderProccssed(robot);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                LoggerHelper.LogAndShowError($"發生錯誤：{e.Message}");
            }
        }

        private async Task<GridTradeRobotOrder> UpdateRunningOrderAsync(OrderResponse order, TradeRobotInfo robot)
        {
            var runningOrder = await _gridTradeRobotOrderRepository.GetRunningOrderByRobotIdAsync(robot.RobotId);

            int adjustmentCount = order.OrderSideStatus == OrderSideStatus.BUY ? 1 : -1;

            //如果有正在執行的訂單
            if (runningOrder != null)
            {
                runningOrder.UpdatedAt = order.UpdateTime;
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
                    CreatedAt = order.UpdateTime
                };
                await _gridTradeRobotOrderRepository.CreateRobotOrderAsync(runningOrder);
                robot.CurrentPositionCount += adjustmentCount;
                robot.StatusEnum = GridTradeRobotStatus.Running;
            }
            return runningOrder;
        }

        private async Task CreateOrderHistoyAsync(OrderResponse order, int rinningOrderId)
        {
            var history = new GridTradeRobotOrderHistory()
            {
                GridTradeRobotOrderId = rinningOrderId,
                Price = order.Price,
                TradeAction = order.OrderSideStatus.ToString(),
                TradeAmount = order.Quantity,
                CreatedAt = order.UpdateTime
            };
            await _gridTradeRobotOrderHostoryRepository.CreateRobotOrderHistoryAsync(history);
        }

        private void UpdateOrderDetail(TradeRobotInfo robot, GridTradeRobotDetail detailInfo)
        {
            detailInfo.AvgPrice = robot.AvgHoldingPrice;
            detailInfo.HoldingAmount = robot.HoldingQty;
            detailInfo.Postions = robot.Postions;
            detailInfo.CurrentPositionCount = robot.CurrentPositionCount;
            _gridTradeRobotDetailRepository.UpdateRobotDetail(detailInfo);
        }
    }
}
