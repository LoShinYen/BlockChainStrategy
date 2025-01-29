using BlockChainStrategy.Library.Models.Dto.Utility;

namespace GridBotStrategy.Services.Strategies
{
    public class LongTradeStrategy : BaseStratgyService, ITradeStrategy
    {

        public async Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot)
        {
            var response = new OrderResponse();
            if (CheckIsOpen(robot.CurrentPositionCount))
            {
                response = await RaisePositionAsync(robot);
            }
            else
            {
                if (CheckPriceIsRaise(robot.LastPrice, robot.CurrentPrice))
                {
                    response = await RaisePositionAsync(robot);
                }
                else
                {
                    response = await ReducePositionAsync(robot);
                }
            }
            return response;
        }
    }
}
