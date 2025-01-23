namespace BlockChainStrategy.Library.Models.Context
{
    public partial class GridTradeRobotDetail
    {
        [NotMapped]
        public List<TradeRobotPosition> Postions
        {
            get => System.Text.Json.JsonSerializer.Deserialize<List<TradeRobotPosition>>(GridInfos) ?? new List<TradeRobotPosition>();
            set => GridInfos = System.Text.Json.JsonSerializer.Serialize(value);
        }

    }
}
