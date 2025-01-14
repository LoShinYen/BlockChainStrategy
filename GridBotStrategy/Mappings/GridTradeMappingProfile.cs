using AutoMapper;

namespace GridBotStrategy.Mappings
{
    internal class GridTradeMappingProfile : Profile
    {
        public GridTradeMappingProfile() 
        {
            CreateMap<GridTradeRobot, TradeRobotDto>()
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiKey)))
                .ForMember(dest => dest.ApiSecret, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiSecret)));
        }
    }
}
