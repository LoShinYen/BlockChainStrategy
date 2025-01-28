using BlockChainStrategy.Library.Enums;

namespace BlockChainStrategy.Library.Models.Dto.Utility
{
    public class ExchangeConfig
    {
        public ExchangeType ExchangeType { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public bool Test { get; set; }
    }
}
