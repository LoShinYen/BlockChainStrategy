namespace BlockChainStrategy.Library.Helpers
{
    internal static class BinanceCalcuteQtyHelper
    {
        /// <summary>
        /// 計算符合 Binance 規範的交易數量
        /// </summary>
        /// <returns>調整後的交易數量</returns>
        /// <exception cref="ArgumentException">如果金額低於最小交易限制或不符合最小單位限制，拋出異常</exception>
        internal static decimal CalcuteQty(OrderRequest request)
        {
            var usdtAmount = request.UsdtQuantity * request.Laverage;
            // 計算理論交易數量
            decimal rawQty = usdtAmount / request.CurrentPrice;

            // 定義每個交易對的最小交易金額限制（USDT）
            decimal minAmount = request.Symbol switch
            {
                "BTCUSDT" => 0.001m,
                "ETHUSDT" => 0.006m,
                "SOLUSDT" => 1m,    
                "BNBUSDT" => 0.01m, 
                "ADAUSDT" => 5m,    
                _ => throw new ArgumentException($"不支持的交易對: {request.Symbol}")
            };

            // 檢查金額是否低於最小交易金額限制
            if (rawQty < minAmount)
            {
                throw new ArgumentException($"金額低於 {request.Symbol} 的最小交易金額: {minAmount} USDT");
            }

            // 定義每個交易對的最小交易單位 (stepSize)
            decimal stepSize = request.Symbol switch
            {
                "BTCUSDT" => 0.001m,
                "ETHUSDT" => 0.001m,   
                "SOLUSDT" => 1m,       
                "BNBUSDT" => 0.01m,    
                "ADAUSDT" => 1m,       
                _ => throw new ArgumentException($"不支持的交易對: {request.Symbol}")
            };

            // 調整到符合最小單位限制的數量
            decimal adjustedQty = Math.Floor(rawQty / stepSize) * stepSize;

            return adjustedQty;
        }
    }
}
