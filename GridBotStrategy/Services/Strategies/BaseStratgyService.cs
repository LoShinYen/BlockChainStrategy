using BlockChainStrategy.Library.Models.Dto;
using BlockChainStrategy.Library.Enums.Binance;
using BlockChainStrategy.Library.Models.Dto.Binance;

namespace GridBotStrategy.Services
{
    public class BaseStratgyService
    {
        /// <summary>
        /// 檢查是否有開倉
        /// </summary>
        /// <param name="currentPositionCount">目前倉位數量</param>
        /// <returns></returns>
        public bool CheckIsOpen(int currentPositionCount)
        {
            if (currentPositionCount > 0) return true;
            return false;
        }

        /// <summary>
        /// 檢查價格是否上漲
        /// </summary>
        /// <param name="lastPrice">上次價錢</param>
        /// <param name="currentPrice">市價</param>
        /// <returns></returns>
        public bool CheckPriceIsRaise(decimal lastPrice, decimal currentPrice)
        {
            return lastPrice < currentPrice;
        }

        /// <summary>
        /// 加倉
        /// </summary>
        /// <param name="robot"></param>
        /// <returns></returns>
        public async Task RaisePositionAsync(TradeRobotInfo robot)
        {
            //var binacneHelper = new BinanceHelper(robot.ApiKey, robot.ApiSecret, true);

            //await binacneHelper.ChangePositionModeAsync(true);

            //var laverge = new BinanceChangeLeverageRequestDto() { Leverage = robot.Laverage, Symbol = robot.Symbol };
            //await binacneHelper.ChangeLeverageAsync(laverge);

            //var createOrder = new BinanceCreateOrderRequestDto()
            //{
            //    Symbol = robot.Symbol,
            //    Side = OrderSide.BUY,
            //    Quantity = robot.PerTradeAmountUSDT,
            //};
            //await binacneHelper.CreateOrderAsync(createOrder);
        }

        /// <summary>
        /// 減倉
        /// </summary>
        /// <param name="robot"></param>
        /// <returns></returns>
        public async Task ReducePositionAsync(TradeRobotInfo robot)
        {
            //var binacneHelper = new BinanceHelper(robot.ApiKey, robot.ApiSecret, true);
            //var createOrder = new BinanceCreateOrderRequestDto()
            //{
            //    Symbol = robot.Symbol,
            //    Side = OrderSide.BUY,
            //    Quantity = robot.ReduceQty,
            //};
            //await binacneHelper.CreateOrderAsync(createOrder);
        }
    }
}
