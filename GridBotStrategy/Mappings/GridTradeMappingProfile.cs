using System.Data;

namespace GridBotStrategy.Mappings
{
    internal class GridTradeMappingProfile : Profile
    {
        public GridTradeMappingProfile() 
        {
            CreateMap<GridTradeRobot, TradeRobotInfo>()
                .ForMember(dest => dest.RobotId, opt => opt.MapFrom(src => src.GridTradeRobotId))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.CurrentPositionCount , opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().CurrentPositionCount))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiKey)))
                .ForMember(dest => dest.ApiSecret, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiSecret)))
                .ForMember(dest => dest.Postions, opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().Postions));

        }
    }
}
