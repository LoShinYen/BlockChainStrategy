using BlockChainStrategy.Library.Models;
using GridBotStrategy.Observers;
using GridBotStrategy.Strategies;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        private Dictionary<string,decimal> _symbolMarkPrice = new Dictionary<string, decimal>();
        private readonly GridTradeRobotRepository _robotRepository;

        public TradeExecutionService(GridTradeRobotRepository robotRepository)
        {
            _robotRepository = robotRepository;
        }

        public void OnMarketDataReceived(BinanceMarketPriceData message)
        {
            UpdateMarketPrice(message);
        }

        private void UpdateMarketPrice(BinanceMarketPriceData message)
        {
            decimal.TryParse(message.MarkPrice, out var price);
            if (_symbolMarkPrice.ContainsKey(message.Symbol))
            {
                _symbolMarkPrice[message.Symbol] = price;
            }
            else
            {
                _symbolMarkPrice.Add(message.Symbol, price);
            }
        }

        public async Task ExcuteTradeAsync()
        {
            while(true)
            {
                var robots = await _robotRepository.GetAllRobotsAsync();
                foreach (var robot in robots)
                {
                    if (_symbolMarkPrice.TryGetValue(robot.Symbol, out var currentMarketPrice))
                    {
                        var strategy = TradeStrategyFactory.GetStrategy(robot.PositionSideEnum);
                        await strategy.ExecuteTradeAsync(robot, currentMarketPrice);
                    }

                }

            }
        }

    }
}
