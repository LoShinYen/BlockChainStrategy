namespace BlockChainStrategy.Library.Models.Dto.Binance
{
    public class BinanceChangePositionModeResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 回應訊息
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; } = string.Empty;
    }
}
