using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Enums.GridRobot;

namespace BlockChainStrategy.Library.Models.Context
{
    public partial class GridTradeRobot
    {
        [NotMapped]
        public GridTradeRobotStatus StatusEnum
        {
            get => Enum.Parse<GridTradeRobotStatus>(Status);
            set => Status = value.ToString();
        }

        [NotMapped]
        public GridTradeRobotPositionSide PositionSideEnum
        {
            get => Enum.Parse<GridTradeRobotPositionSide>(PositionSide);
            set => PositionSide = value.ToString();
        }

        [NotMapped]
        public ExchangeType ExchangeTypeEnum
        {
            get => Enum.Parse<ExchangeType>(Exchange);
            set => Exchange = value.ToString();
        }

    }
}
