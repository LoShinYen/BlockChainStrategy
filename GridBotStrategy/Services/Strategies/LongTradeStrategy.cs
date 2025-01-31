using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Helpers.Interface;

namespace GridBotStrategy.Services.Strategies
{
    public class LongTradeStrategy : BaseStrategyService, ITradeStrategy
    {
        public LongTradeStrategy(IExchangeClientFactory exchangeClientFactory) : base(exchangeClientFactory)
        {
        }

        public async Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot)
        {
            var response = new OrderResponse();
            if (CheckIsOpenAction(robot.CurrentPositionCount))
            {
                response = await RaisePositionAsync(robot);
            }
            else
            {
                if (CheckPriceIsRaise(robot.LastPrice, robot.CurrentPrice))
                {
                    response = await ReducePositionAsync(robot);
                }
                else
                {
                    response = await RaisePositionAsync(robot);
                }
            }
            return response;
        }

        public void UpdatePositionInfo(OrderResponse order, TradeRobotInfo robot)
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

            //Rest Robot Postions IsLastTarget
            robot.Postions.ForEach(p => p.IsLastTarget = false);
            robot.Postions[robot.TargetPositionIndex].IsLastTarget = true;
        }

    }
}
