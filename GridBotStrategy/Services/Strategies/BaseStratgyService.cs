using BlockChainStrategy.Library.Models.Dto;
using BlockChainStrategy.Library.Enums.Binance;

namespace GridBotStrategy.Services
{
    public class BaseStratgyService
    {
        public bool CheckIsOpen(int currentPositionCount)
        {
            if (currentPositionCount > 0) return true;
            return false;
        }

        public async Task OpenPositionAsync(TradeRobotInfo robot)
        {
            var binacneHelper = new BinanceHelper(robot.ApiKey,robot.ApiSecret,true);
         
            await binacneHelper.ChangePositionModeAsync(true);

            var laverge = new BinanceChangeLeverageRequestDto() { Leverage = robot.Laverage , Symbol = robot.Symbol };
            await binacneHelper.ChangeLeverageAsync(laverge);

            var createOrder = new BinanceCreateOrderRequestDto()
            {
                Symbol = robot.Symbol,
                Side = OrderSide.BUY,
                Quantity = robot.PerTradeAmountUSDT,
            };
            await binacneHelper.CreateOrderAsync(createOrder);
        }


        public async Task RaisePositionAsync(TradeRobotInfo robot)
        {
            // Raise Position
        }

        public async Task ClosePositionAsync(TradeRobotInfo robot)
        {
            // Close Position
        }

        public bool CheckPriceIsRaise(decimal lastPrice, decimal currentPrice)
        {
            return lastPrice < currentPrice;
        }

    }
}
