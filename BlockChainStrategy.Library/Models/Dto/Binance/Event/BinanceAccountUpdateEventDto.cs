namespace BlockChainStrategy.Library.Models.Dto.Binance.Event
{
    /// <summary>
    /// e = "ACCOUNT_UPDATE" 帳戶餘額與持倉更新
    /// </summary>
    public class BinanceAccountUpdateEvent
    {
        /// <summary>
        /// 事件類型，固定為 "ACCOUNT_UPDATE"
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// 撮合時間 (交易引擎執行時間)
        /// </summary>
        [JsonProperty("T")]
        public long TransactionTime { get; set; }

        /// <summary>
        /// 事件時間
        /// </summary>
        [JsonProperty("E")]
        public long EventTime { get; set; }

        /// <summary>
        /// 帳戶更新內容
        /// </summary>
        [JsonProperty("a")]
        public AccountUpdateData Data { get; set; } = new AccountUpdateData();
    }

    public class AccountUpdateData
    {
        /// <summary>
        /// 事件原因類型，如 "ORDER", "FUNDING_FEE", ...
        /// </summary>
        [JsonProperty("m")]
        public string EventReasonType { get; set; } = string.Empty;

        /// <summary>
        /// 資產餘額清單
        /// </summary>
        [JsonProperty("B")]
        public List<BalanceData> Balances { get; set; } = new List<BalanceData>();

        /// <summary>
        /// 持倉資訊清單
        /// </summary>
        [JsonProperty("P")]
        public List<PositionData> Positions { get; set; } = new List<PositionData>();
    }

    public class BalanceData
    {
        /// <summary>
        /// 資產種類, 如 "USDT"
        /// </summary>
        [JsonProperty("a")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// 錢包餘額 (wallet balance)
        /// </summary>
        [JsonProperty("wb")]
        public string WalletBalance { get; set; } = string.Empty;

        /// <summary>
        /// Cross Wallet 或者 總的Cross保證金
        /// </summary>
        [JsonProperty("cw")]
        public string CrossWalletBalance { get; set; } = string.Empty;

        /// <summary>
        /// 變動金額 (bc)
        /// </summary>
        [JsonProperty("bc")]
        public string BalanceChange { get; set; } = string.Empty;
    }

    public class PositionData
    {
        /// <summary>
        /// 交易對，如 "BTCUSDT"
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 持倉數量 (正多頭/負空頭)
        /// </summary>
        [JsonProperty("pa")]
        public string PositionAmount { get; set; } = string.Empty;

        /// <summary>
        /// 入場價格 (entry price)
        /// </summary>
        [JsonProperty("ep")]
        public string EntryPrice { get; set; } = string.Empty;

        /// <summary>
        /// 已實現盈虧 (cr)
        /// </summary>
        [JsonProperty("cr")]
        public string RealizedPnl { get; set; } = string.Empty;

        /// <summary>
        /// 未實現盈虧 (up)
        /// </summary>
        [JsonProperty("up")]
        public string UnrealizedPnl { get; set; } = string.Empty;

        /// <summary>
        /// 保證金類型，如 "cross" 或 "isolated"
        /// </summary>
        [JsonProperty("mt")]
        public string MarginType { get; set; } = string.Empty;

        /// <summary>
        /// 若為 isolated 時, isolated wallet
        /// </summary>
        [JsonProperty("iw")]
        public string IsolatedWallet { get; set; } = string.Empty;

        /// <summary>
        /// 倉位方向，如 "LONG", "SHORT", "BOTH"
        /// </summary>
        [JsonProperty("ps")]
        public string PositionSide { get; set; } = string.Empty;

        /// <summary>
        /// 保證金資產
        /// </summary>
        [JsonProperty("ma")]
        public string MarginAsset { get; set; } = string.Empty;

        /// <summary>
        /// Break-Even Price 或其他資訊 (bep)
        /// </summary>
        [JsonProperty("bep")]
        public string BreakEvenPrice { get; set; } = string.Empty;
    }
}
