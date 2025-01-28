namespace BlockChainStrategy.Library.Models.Dto.Binance.Request
{
    public class BinanceChangeLeverageRequestDto
    {
        /// <summary>
        /// 交易對，例如 BTCUSDT
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 新的槓桿倍數，介於 1 到 125
        /// </summary>
        [JsonProperty("leverage")]
        public int Laverage { get; set; }

        /// <summary>
        /// 當前時間戳
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}
