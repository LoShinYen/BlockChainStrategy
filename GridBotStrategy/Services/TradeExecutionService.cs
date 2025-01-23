using BlockChainStrategy.Library.Models.Dto.Binance;
using GridBotStrategy.Observers;
using System.Threading.Channels;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        private readonly IGridTradeRobotRepository _robotRepository;
        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private readonly ITradeHandler _tradeHandler;

        public TradeExecutionService(IGridTradeRobotRepository robotRepository, IMapper mapper , IMarketDataService marketDataService, ITradeHandler tradeHandler)
        {
            _robotRepository = robotRepository;
            _mapper = mapper;
            _marketDataService = marketDataService;
            _tradeHandler = tradeHandler;
        }

        public void OnMarketDataReceived(BinanceMarketPriceDataDto message)
        {
            _marketDataService.UpdateMarketPrice(message);
        }


        public async Task ExcuteTradeAsync()
        {
            List<TradeRobotInfo> robots = await LoadRobotsAsync();

            Channel<TradeRobotInfo> channel = CreateChannel(robots);

            IEnumerable<Task> consumers = CreateConsumer(channel);

            await Task.WhenAll(consumers);
        }

        private async Task<List<TradeRobotInfo>> LoadRobotsAsync()
        {
            var robotDbInfo = await _robotRepository.GetRunningRobotsAsync();
            var robots = _mapper.Map<List<TradeRobotInfo>>(robotDbInfo);
            return robots;
        }

        private static Channel<TradeRobotInfo> CreateChannel(List<TradeRobotInfo> robots)
        {
            var channel = Channel.CreateUnbounded<TradeRobotInfo>();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var robot in robots)
                    {
                        await channel.Writer.WriteAsync(robot);
                    }
                    await Task.Delay(500);
                }
            });
            return channel;
        }

        private IEnumerable<Task> CreateConsumer(Channel<TradeRobotInfo> channel)
        {
            return Enumerable.Range(0, 5).Select(async _ =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var robot))
                    {
                        await _tradeHandler.HandleTradeAsync(robot);
                    }
                }
            });
        }
    }
}
