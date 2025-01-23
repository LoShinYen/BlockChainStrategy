namespace BlockChainStrategy.Library.Models.Dto.Binance.Event
{
    /// <summary>
    /// 對應 e = "TRADE_LITE" 的事件結構
    /// </summary>
    public class BinanceTradeLiteEvent
    {
        /// <summary>
        /// 事件類型，固定為 "TRADE_LITE"
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// 事件時間 (ms)
        /// </summary>
        [JsonProperty("E")]
        public long EventTime { get; set; }

        /// <summary>
        /// 交易時間 (ms)
        /// </summary>
        [JsonProperty("T")]
        public long TradeTime { get; set; }

        /// <summary>
        /// 交易對，如 "BTCUSDT"
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 成交數量 (或下單數量) ，如 "0.005"
        /// </summary>
        [JsonProperty("q")]
        public string Quantity { get; set; } = string.Empty;

        /// <summary>
        /// 當前(委託)價格, 若為 "0.00" 代表市價?
        /// </summary>
        [JsonProperty("p")]
        public string Price { get; set; } = string.Empty;

        /// <summary>
        /// 是否是Maker單 (true/false)
        /// </summary>
        [JsonProperty("m")]
        public bool IsMaker { get; set; }

        /// <summary>
        /// 自定義訂單號 (clientOrderId)
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// 買賣方向，如 "BUY" 或 "SELL"
        /// </summary>
        [JsonProperty("S")]
        public string Side { get; set; } = string.Empty;

        /// <summary>
        /// 限價 (L)
        /// </summary>
        [JsonProperty("L")]
        public string LimitPrice { get; set; } = string.Empty;

        /// <summary>
        /// 最後成交量 (l)
        /// </summary>
        [JsonProperty("l")]
        public string LastFilledQuantity { get; set; } = string.Empty;

        /// <summary>
        /// 交易ID (t)
        /// </summary>
        [JsonProperty("t")]
        public long TradeId { get; set; }

        /// <summary>
        /// 系統訂單ID (i)
        /// </summary>
        [JsonProperty("i")]
        public long OrderId { get; set; }
    }
}
