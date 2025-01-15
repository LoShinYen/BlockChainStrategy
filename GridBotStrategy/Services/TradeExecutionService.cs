using AutoMapper;
using BlockChainStrategy.Library.Models;
using GridBotStrategy.Observers;
using System.Threading.Channels;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        private Dictionary<string,decimal> _symbolMarkPrice = new Dictionary<string, decimal>();
        private readonly IGridTradeRobotRepository _robotRepository;
        private readonly IMapper _mapper;

        public TradeExecutionService(IGridTradeRobotRepository robotRepository, IMapper mapper)
        {
            _robotRepository = robotRepository;
            _mapper = mapper;
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
            var robotDbInfo = await _robotRepository.GetRunningRobotsAsync();
            var robots = _mapper.Map<List<TradeRobotInfo>>(robotDbInfo);

            var channel = Channel.CreateUnbounded<TradeRobotInfo>();

            _ = Task.Run(async () =>
            {
                while (true)
                {

                    foreach (var robot in robots)
                    {
                        await channel.Writer.WriteAsync(robot);
                    }
                    await Task.Delay(1000);
                }
            });

            var consumers = Enumerable.Range(0, 5).Select(async _ =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var robot))
                    {
                        try
                        {
                            if (_symbolMarkPrice.TryGetValue(robot.Symbol, out var currentMarketPrice))
                            {


                                //var strategy = TradeStrategyFactory.GetStrategy(robot.PositionSideEnum);
                                //await strategy.ExecuteTradeAsync(robot, currentMarketPrice);
                                robot.LastPrice = currentMarketPrice;
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.LogError($"Robot Id: {robot.RobotId}, {ex.Message}");
                        }
                    }
                }
            });

            await Task.WhenAll(consumers);
        }

        private bool CheckTargetPrice(TradeRobotInfo robot , decimal currentPrice)
        {



            return false;
        }
    }
}
