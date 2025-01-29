using BlockChainStrategy.Library.Models.Dto.Utility;

namespace GridBotStrategy.Services.Strategies
{
    public interface ITradeStrategy
    {
        Task<OrderResponse> ExecuteTradeAsync(TradeRobotInfo robot);
    }
}
