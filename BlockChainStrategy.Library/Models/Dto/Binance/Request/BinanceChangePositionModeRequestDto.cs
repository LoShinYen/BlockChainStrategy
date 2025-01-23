namespace BlockChainStrategy.Library.Models.Dto.Binance
{
    public class BinanceChangePositionModeRequestDto
    {
        /// <summary>
        /// 是否切換為雙向持倉模式
        /// </summary>
        [JsonProperty("dualSidePosition")]
        public bool DualSidePosition { get; set; }

        /// <summary>
        /// 當前時間戳
        /// </summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
