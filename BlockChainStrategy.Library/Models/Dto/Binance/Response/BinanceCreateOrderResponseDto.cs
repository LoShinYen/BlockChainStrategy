namespace BlockChainStrategy.Library.Models.Dto.Binance
{
    public class BinanceCreateOrderResponseDto
    {
        /// <summary>
        /// 使用者自定義的訂單編號
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// 累計成交數量
        /// </summary>
        [JsonProperty("cumQty")]
        public string CumQty { get; set; } = string.Empty;

        /// <summary>
        /// 累計成交金額
        /// </summary>
        [JsonProperty("cumQuote")]
        public string CumQuote { get; set; } = string.Empty;

        /// <summary>
        /// 已成交量
        /// </summary>
        [JsonProperty("executedQty")]
        public string ExecutedQty { get; set; } = string.Empty;

        /// <summary>
        /// 系統生成的訂單編號
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// 平均成交價格
        /// </summary>
        [JsonProperty("avgPrice")]
        public string AvgPrice { get; set; } = string.Empty;

        /// <summary>
        /// 原始委託數量
        /// </summary>
        [JsonProperty("origQty")]
        public string OrigQty { get; set; } = string.Empty;

        /// <summary>
        /// 委託價格
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; } = string.Empty;

        /// <summary>
        /// 僅減倉標誌
        /// </summary>
        [JsonProperty("reduceOnly")]
        public bool ReduceOnly { get; set; }

        /// <summary>
        /// 買賣方向 (BUY/SELL)
        /// </summary>
        [JsonProperty("side")]
        public string Side { get; set; } = string.Empty;

        /// <summary>
        /// 持倉方向 (LONG/SHORT)
        /// </summary>
        [JsonProperty("positionSide")]
        public string PositionSide { get; set; } = string.Empty;

        /// <summary>
        /// 訂單狀態
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 觸發價格 (對 TRAILING_STOP_MARKET 無效)
        /// </summary>
        [JsonProperty("stopPrice")]
        public string StopPrice { get; set; } = string.Empty;

        /// <summary>
        /// 條件全平倉標誌
        /// </summary>
        [JsonProperty("closePosition")]
        public bool ClosePosition { get; set; }

        /// <summary>
        /// 交易對 (如 BTCUSDT)
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 有效方式 (如 GTC、GTD)
        /// </summary>
        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; } = string.Empty;

        /// <summary>
        /// 訂單類型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 觸發前的訂單類型
        /// </summary>
        [JsonProperty("origType")]
        public string OrigType { get; set; } = string.Empty;

        /// <summary>
        /// 跟蹤止損觸發價格
        /// </summary>
        [JsonProperty("activatePrice")]
        public string? ActivatePrice { get; set; }

        /// <summary>
        /// 跟蹤止損回調比例
        /// </summary>
        [JsonProperty("priceRate")]
        public string? PriceRate { get; set; }

        /// <summary>
        /// 更新時間 (Unix 時間戳)
        /// </summary>
        [JsonProperty("updateTime")]
        public long UpdateTime { get; set; }

        /// <summary>
        /// 條件價格觸發方式
        /// </summary>
        [JsonProperty("workingType")]
        public string WorkingType { get; set; } = string.Empty;

        /// <summary>
        /// 是否啟用條件單觸發保護
        /// </summary>
        [JsonProperty("priceProtect")]
        public bool PriceProtect { get; set; }

        /// <summary>
        /// 盘口价格下单模式
        /// </summary>
        [JsonProperty("priceMatch")]
        public string PriceMatch { get; set; } = string.Empty;

        /// <summary>
        /// 自成交保護模式
        /// </summary>
        [JsonProperty("selfTradePreventionMode")]
        public string SelfTradePreventionMode { get; set; } = string.Empty;

        /// <summary>
        /// 當 TimeInForce 為 GTD 時，自動取消的時間戳
        /// </summary>
        [JsonProperty("goodTillDate")]
        public long? GoodTillDate { get; set; }
    }
}
