namespace GridBotStrategy.Models
{
    public class TradeRobotInfo
    {
        public int RobotId { get; set; }

        public string Symbol { get; set; } = string.Empty;

        public List<TradeRobotPosition> Postions { get; set; } = new List<TradeRobotPosition>();

        public string ApiKey { get; set; } = string.Empty;

        public string ApiSecret { get; set; } = string.Empty;

        /// <summary>
        /// 機器人狀態
        /// </summary>
        public GridTradeRobotStatus StatusEnum { get; set; }

        /// <summary>
        /// 機器人倉位方向
        /// </summary>
        public GridTradeRobotPositionSide PositionSideEnum { get; set; }

        /// <summary>
        /// 上次市價
        /// </summary>
        public decimal LastPrice { get; set; }

        public decimal CurrentPrice { get; set; }


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

    }
}
