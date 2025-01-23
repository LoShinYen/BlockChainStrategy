namespace BlockChainStrategy.Library.Models.Dto.Binance.Response
{
    public class BinanceErrorMsgResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; } = string.Empty;

    }
}
