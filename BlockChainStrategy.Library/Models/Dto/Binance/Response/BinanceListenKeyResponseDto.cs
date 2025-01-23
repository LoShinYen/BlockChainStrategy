namespace BlockChainStrategy.Library.Models.Dto.Binance
{
    public class BinanceListenKeyResponseDto
    {
        [JsonProperty("listenKey")]
        public string ListenKey { get; set; } = string.Empty;
    }
}
