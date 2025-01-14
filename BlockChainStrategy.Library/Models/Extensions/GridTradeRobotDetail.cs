using GridBotStrategy.Models;
using System.Text.Json;

namespace BlockChainStrategy.Library.Models.Context
{
    public partial class GridTradeRobotDetail
    {
        [NotMapped]
        public List<TradeRobotPosition> Postions
        {
            get => JsonSerializer.Deserialize<List<TradeRobotPosition>>(GridInfos) ?? new List<TradeRobotPosition>();
            set => GridInfos = JsonSerializer.Serialize(value);
        }

    }

}
