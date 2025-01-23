using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Enums.Binance;

namespace BlockChainStrategy.Library.Models.Dto.Binance.Request
{
    public class BinanceCreateOrderRequestDto
    {
        /// <summary>
        /// 交易對，例如 BTCUSDT
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 買賣方向
        /// </summary>
        [JsonProperty("side")]
        public OrderSideStatus Side { get; set; }

        /// <summary>
        /// 持倉方向（雙向持倉模式下使用）
        /// </summary>
        [JsonProperty("positionSide")]
        public PositionSideType PositionSide { get; set; } = PositionSideType.LONG;

        /// <summary>
        /// 訂單類型
        /// </summary>
        [JsonProperty("type")]
        public OrderType Type { get; set; } = OrderType.MARKET;

        /// <summary>
        /// 訂單數量
        /// </summary>
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 訂單價格（僅適用於限價單）
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// 時間有效性策略，例如 GTC, IOC, FOK , 默認 GTC
        /// </summary>
        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; } = "GTC";

        /// <summary>
        /// 客戶端自定義訂單標識符
        /// </summary>
        [JsonProperty("newClientOrderId")]
        public string? NewClientOrderId { get; set; }

        /// <summary>
        /// 減倉標誌，默認為 false
        /// </summary>
        [JsonProperty("reduceOnly")]
        public bool? ReduceOnly { get; set; }

        /// <summary>
        /// 止損價格（適用於止損單）
        /// </summary>
        [JsonProperty("stopPrice")]
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// 激活價格（僅適用於觸發條件單，例如 STOP）
        /// </summary>  
        [JsonProperty("activationPrice")]
        public decimal? ActivationPrice { get; set; }

        /// <summary>
        /// 回調比例（僅適用於觸發條件單）
        /// </summary>
        [JsonProperty("callbackRate")]
        public decimal? CallbackRate { get; set; }

    }
}
