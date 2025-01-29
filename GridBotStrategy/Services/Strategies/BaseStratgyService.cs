using BlockChainStrategy.Library.Enums;

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
            if (currentPositionCount > 0) return false;
            return true;
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
        public async Task<OrderResponse> RaisePositionAsync(TradeRobotInfo robot)
        {
            var exchangeConfig = new ExchangeConfig() 
            {
                ApiKey = robot.ApiKey ,
                ApiSecret = robot.ApiSecret ,
                ExchangeType =  robot.ExchangeTypeEnum ,
                Test = true
            };

            var orderRequest = new OrderRequest()
            {
                Symbol = robot.Symbol,
                Side = OrderSideStatus.BUY,
                UsdtQuantity = robot.PerTradeAmountUSDT,
                CurrentPrice = robot.CurrentPrice,
                Laverage = robot.Laverage,
            };

            var exchangeClient = ExchangeFactory.GetExchangeClient(exchangeConfig);

            await exchangeClient.ListenWebSocketAsync();

            var response = await exchangeClient.CreateOrderProcessAsync(orderRequest);
            return response;
        }

        /// <summary>
        /// 減倉
        /// </summary>
        /// <param name="robot"></param>
        /// <returns></returns>
        public async Task<OrderResponse> ReducePositionAsync(TradeRobotInfo robot)
        {

            var exchangeConfig = new ExchangeConfig()
            {
                ApiKey = robot.ApiKey,
                ApiSecret = robot.ApiSecret,
                ExchangeType = robot.ExchangeTypeEnum,
                Test = true
            };

            var orderRequest = new OrderRequest()
            {
                Symbol = robot.Symbol,
                Side = OrderSideStatus.SELL,
                UsdtQuantity = robot.ReduceQty,
                Laverage = robot.Laverage,
            };

            var exchangeClient = ExchangeFactory.GetExchangeClient(exchangeConfig);

            await exchangeClient.ListenWebSocketAsync();
            var response = await exchangeClient.CreateOrderProcessAsync(orderRequest);
            return response;
        }
    }
}
