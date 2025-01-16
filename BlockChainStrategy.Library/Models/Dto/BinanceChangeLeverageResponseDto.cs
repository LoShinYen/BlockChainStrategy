namespace BlockChainStrategy.Library.Models.Dto
{
    public class BinanceChangeLeverageResponseDto
    {
        /// <summary>
        /// 交易對
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 已調整的槓桿倍數
        /// </summary>
        [JsonProperty("leverage")]
        public int Leverage { get; set; }

        /// <summary>
        /// 可用資金
        /// </summary>
        [JsonProperty("maxNotionalValue")]
        public string MaxNotionalValue { get; set; } = string.Empty;
    }
}
