using BlockChainStrategy.Library.Models.Context;
using GridBotStrategy.Helpers.TradeStrategyFactory;
using GridBotStrategy.Repository.Interface;
using GridBotStrategy.Services.Interface;
using Moq;

namespace GridBotStrategyTest.Services.Handlers
{
    internal class TradeHandlerTests
    {
        private readonly Mock<IMarketDataService> _marketDataServiceMock;
        private readonly Mock<IStrategyFactory> _mockFactory;
        private readonly Mock<CryptoPlatformDbContext> _context;
        private readonly Mock<IGridTradeRobotRepository> _gridRobotRepository;
        private readonly Mock<IGridTradeRobotDetailRepository> _gridTradeRobotDetailRepository;
        private readonly Mock<IGridTradeRobotOrderRepository> _gridTradeRobotOrderRepository;
        private readonly Mock<IGridTradeRobotOrderHostoryRepository> _gridTradeRobotOrderHostoryRepository;


        public TradeHandlerTests()
        {
            _marketDataServiceMock = new Mock<IMarketDataService>();
            _mockFactory = new Mock<IStrategyFactory>();
            _context = new Mock<CryptoPlatformDbContext>();
            _gridRobotRepository = new Mock<IGridTradeRobotRepository>();
            _gridTradeRobotDetailRepository = new Mock<IGridTradeRobotDetailRepository>();
            _gridTradeRobotOrderRepository = new Mock<IGridTradeRobotOrderRepository>();
            _gridTradeRobotOrderHostoryRepository = new Mock<IGridTradeRobotOrderHostoryRepository>();
        }

    }
}
