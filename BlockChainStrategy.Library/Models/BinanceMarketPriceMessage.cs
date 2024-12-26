using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlockChainStrategy.Library.Models
{


    public class BinanceMarketPriceMessage
    {
        public string Stream { get; set; } = string.Empty;

        public BinanceMarketPriceData Data { get; set; } = new BinanceMarketPriceData();

    }

    public class BinanceMarketPriceData
    {
        /// <summary>
        /// 事件類型
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; }  = string.Empty;

        /// <summary>
        /// 事件時間
        /// </summary>
        [JsonProperty("E")]
        public long EventTime { get; set; }

        public DateTime EventTimeDateTime => DateTimeOffset.FromUnixTimeMilliseconds(EventTime).DateTime;

        /// <summary>
        /// 交易對
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 標記價格
        /// </summary>
        [JsonProperty("p")]
        public string MarkPrice { get; set; } = string.Empty;

        /// <summary>
        /// 預估結算價格
        /// </summary>
        [JsonProperty("P")]
        public string EstimatedSettlePrice { get; set; } = string.Empty;

        /// <summary>
        /// 指數價格
        /// </summary>
        [JsonProperty("i")]
        public string IndexPrice { get; set; } = string.Empty;

        /// <summary>
        /// 資金費率
        /// </summary>
        [JsonProperty("r")]
        public string FundingRate { get; set; } = string.Empty;

        /// <summary>
        /// 下次資金費用時間
        /// </summary>
        [JsonProperty("T")]
        public long NextFundingTime { get; set; }
    }
}
