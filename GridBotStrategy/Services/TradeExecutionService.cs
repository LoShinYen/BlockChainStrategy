using BlockChainStrategy.Library.Models.Dto.Binance;
using GridBotStrategy.Observers;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace GridBotStrategy.Services
{
    internal class TradeExecutionService : IMarketDataObserver
    {
        private readonly IGridTradeRobotRepository _robotRepository;
        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private readonly ITradeHandler _tradeHandler;
        private readonly int _maxConsumerCount = 5;
        private static ConcurrentDictionary<TradeRobotInfo, bool> _processingRobots = new ConcurrentDictionary<TradeRobotInfo, bool>();

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
            int consumerCount = Math.Min(robots.Count, _maxConsumerCount);

            Channel<TradeRobotInfo> channel = CreateChannel(robots);

            IEnumerable<Task> consumers = CreateConsumer(channel, consumerCount);
            
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
            var channel = Channel.CreateBounded<TradeRobotInfo>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait
            });

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var robot in robots)
                    {
                        if (_processingRobots.TryAdd(robot, true))
                        {
                            await channel.Writer.WriteAsync(robot);
                        }
                    }
                    await Task.Delay(500);
                }
            });

            return channel;
        }

        private IEnumerable<Task> CreateConsumer(Channel<TradeRobotInfo> channel, int consumerCount)
        {
            return Enumerable.Range(0, consumerCount).Select(async _ =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var robot))
                    {
                        try
                        {
                            await _tradeHandler.HandleTradeAsync(robot);
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.LogAndShowError($"發生錯誤：{ex.Message}");
                        }
                        finally
                        {
                            if (!_processingRobots.TryRemove(robot, out bool finish))
                            {
                                LoggerHelper.LogAndShowError($"移除處理中的機器人失敗：{robot}");
                            }
                        }
                    }
                }
            });
        }
    }
}
