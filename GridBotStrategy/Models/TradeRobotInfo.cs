namespace GridBotStrategy.Models
{
    internal class TradeRobotInfo
    {
        internal int RobotId { get; set; }

        internal string Symbol { get; set; } = string.Empty;

        internal List<TradeRobotPosition> Postions { get; set; } = new List<TradeRobotPosition>();

        //internal 

        internal string ApiKey { get; set; } = string.Empty;

        internal string ApiSecret { get; set; } = string.Empty;

        internal GridTradeRobotStatus StatusEnum { get; set; }

        internal GridTradeRobotPositionSide PositionSideEnum { get; set; }

        internal decimal LastPrice { get; set; }

    }
}
