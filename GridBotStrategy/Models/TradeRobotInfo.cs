using BlockChainStrategy.Library.Models.Dto.Utility;

namespace GridBotStrategy.Models
{
    public class TradeRobotInfo
    {
        public int RobotId { get; set; }

        /// <summary>
        /// 槓桿倍數
        /// </summary>
        public int Laverage { get; set; } = 1;

        /// <summary>
        /// 交易對
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 機器人總金額
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 每注下單金額(USDT)
        /// </summary>
        public decimal PerTradeAmountUSDT => Postions.Count == 0 ? 0 : TotalAmount / Postions.Count;

        /// <summary>
        /// 每次減倉單位
        /// </summary>
        public decimal ReduceQty => CurrentPositionCount == 0 ? 0 : HoldingQty / CurrentPositionCount;

        /// <summary>
        /// 持有貨幣數量
        /// </summary>
        public decimal HoldingQty { get; set; }

        /// <summary>
        /// 倉位資訊
        /// </summary>
        public List<TradeRobotPosition> Postions { get; set; } = new List<TradeRobotPosition>();

        /// <summary>
        /// 機器人狀態
        /// </summary>
        public GridTradeRobotStatus StatusEnum { get; set; }

        /// <summary>
        /// 機器人倉位方向
        /// </summary>
        public GridTradeRobotPositionSide PositionSideEnum { get; set; }

        /// <summary>
        /// 上次觸發目標價格
        /// </summary>
        public decimal LastTargetPositionPrice { get; set; }

        /// <summary>
        /// 觸發倉位價格Index
        /// </summary>
        public int TargetPositionIndex { get; set; } = -1;

        /// <summary>
        /// 機器人開倉次數
        /// </summary>
        public int CurrentPositionCount { get; set; } = 0;

        /// <summary>
        /// 當前市價
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 上次市價
        /// </summary>
        public decimal LastPrice { get; set; }

        public string ApiKey { get; set; } = string.Empty;

        public string ApiSecret { get; set; } = string.Empty;

    }
}
