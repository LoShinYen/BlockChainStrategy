namespace GridBotStrategy.Models
{
    internal class TradeRobotDto
    {
        internal int GridTradeRobotId { get; set; }

        internal string Symbol { get; set; } = string.Empty;

        internal TradeRobotPosition Postions { get; set; } = new TradeRobotPosition();

        internal string ApiKey { get; set; } = string.Empty;

        internal string ApiSecret { get; set; } = string.Empty;

        internal GridTradeRobotStatus StatusEnum { get; set; }

        internal GridTradeRobotPositionSide PositionSideEnum { get; set; }

    }
}
