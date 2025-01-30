namespace BlockChainStrategy.Library.Helpers
{
    public static class BinanceCalculateQtyHelper
    {
        /// <summary>
        /// 幫助計算符合 Binance 交易規範的數量(USDT)
        /// </summary>
        /// <returns>調整後的交易數量</returns>
        /// <exception cref="ArgumentException">如果金額低於最小交易限制或不符合最小單位限制，拋出異常</exception>
        public static decimal CalculateValidTradeQuantity(OrderRequest request)
        {
            var usdtAmount = request.UsdtQuantity * request.Laverage;
            // 計算理論交易數量
            decimal rawQty = usdtAmount / request.CurrentPrice;

            // 調整到符合最小單位限制的數量
            decimal validQuantity = AdjustQuantityToStepSize(request.Symbol, rawQty);

            return validQuantity;
        }

        /// <summary>
        /// 調整交易數量，使其符合 Binance 交易規範（最小交易金額與最小交易單位）
        /// </summary>
        /// <param name="symbol">指定貨幣</param>
        /// <param name="quantity">該貨幣數量</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static decimal AdjustQuantityToStepSize(string symbol, decimal quantity)
        {
            // 定義每個交易對的最小交易金額限制（USDT）
            decimal minTradeAmount = symbol switch
            {
                "BTCUSDT" => 0.001m,
                "ETHUSDT" => 0.006m,
                "SOLUSDT" => 1m,
                "BNBUSDT" => 0.01m,
                "ADAUSDT" => 5m,
                _ => throw new ArgumentException($"不支持的交易對: {symbol}")
            };

            // 檢查金額是否低於最小交易金額限制
            if (quantity < minTradeAmount)
            {
                throw new ArgumentException($"金額低於 {symbol} 的最小交易金額: {minTradeAmount} USDT");
            }

            // 定義每個交易對的最小交易單位 (stepSize)
            decimal minStepSize = symbol switch
            {
                "BTCUSDT" => 0.001m,
                "ETHUSDT" => 0.001m,
                "SOLUSDT" => 1m,
                "BNBUSDT" => 0.01m,
                "ADAUSDT" => 1m,
                _ => throw new ArgumentException($"不支持的交易對: {symbol}")
            };

            decimal validQuantity = Math.Floor(quantity / minStepSize) * minStepSize;

            return validQuantity;
        }
    }
}
