namespace GridBotStrategy.Mappings
{
    internal class GridTradeMappingProfile : Profile
    {
        public GridTradeMappingProfile() 
        {
            CreateMap<GridTradeRobot, TradeRobotInfo>()
                .ForMember(dest => dest.RobotId, opt => opt.MapFrom(src => src.GridTradeRobotId))
                .ForMember(dest => dest.ExchangeTypeEnum, opt => opt.MapFrom(src => src.ExchangeTypeEnum))
                .ForMember(dest => dest.Laverage, opt => opt.MapFrom(src => src.Laverage))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.AmountUsdt))
                .ForMember(dest => dest.CurrentPositionCount , opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().CurrentPositionCount))
                .ForMember(dest => dest.ApiKey, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiKey)))
                .ForMember(dest => dest.ApiSecret, opt => opt.MapFrom(src => EncryptionHelper.Decrypt(src.EncryptedApiSecret)))
                .ForMember(dest => dest.Postions, opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().Postions))
                .ForMember(dest => dest.HoldingQty , opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().HoldingAmount))
                .ForMember(dest => dest.AvgHoldingPrice , opt => opt.MapFrom(src => src.GridTradeRobotDetails.First().AvgPrice));
        }
    }
}
