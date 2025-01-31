using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Exchange.Interface;
using BlockChainStrategy.Library.Helpers.Interface;
using BlockChainStrategy.Library.Models.Dto.Utility;
using GridBotStrategy.Models;
using GridBotStrategy.Services;
using Moq;

namespace GridBotStrategyTest.Services.Strategies
{
    public class BaseStratgyServiceTests

    {
        private readonly Mock<IExchangeClientFactory> _mockFactory;
        private readonly Mock<IExchangeClient> _mockClient;
        private readonly BaseStrategyService _service;

        public BaseStratgyServiceTests()
        {
            _mockFactory = new Mock<IExchangeClientFactory>();
            _mockClient = new Mock<IExchangeClient>();

            _mockFactory
                .Setup(f => f.GetExchangeClient(It.IsAny<ExchangeConfig>()))
                .Returns(_mockClient.Object);

            _service = new BaseStrategyService(_mockFactory.Object);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(-2, false)]
        public void CheckIsOpen_ReturnsExpected(int currentPos, bool expected)
        {
            var result = _service.CheckIsOpenAction(currentPos);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(100, 101, true)]
        [InlineData(100, 100, false)]
        [InlineData(101, 100, false)]
        public void CheckPriceIsRaise_ReturnsExpected(decimal lastPrice, decimal currPrice, bool expected)
        {
            var result = _service.CheckPriceIsRaise(lastPrice, currPrice);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task RaisePositionAsync_ShouldCreateOrderProcessWithBuySide()
        {
            // Arrange
            var robot = new TradeRobotInfo
            {
                Symbol = "BTCUSDT",
                TotalAmount = 100,
                CurrentPrice = 20000,
                Laverage = 10
            };
            for (int i = 1; i <= 2; i++)
            { 
                robot.Postions.Add(new TradeRobotPosition {});
            }

            var expectedResponse = new OrderResponse { Price = 20000, Quantity = 0.0025m };
            _mockClient
                .Setup(c => c.CreateOrderProcessAsync(It.IsAny<OrderRequest>(), true))
                .ReturnsAsync(expectedResponse);

            // Act
            var actual = await _service.RaisePositionAsync(robot);

            // Assert
            Assert.Equal(expectedResponse, actual);

            // 驗證呼叫時的參數
            _mockClient.Verify(client => client.CreateOrderProcessAsync(
                It.Is<OrderRequest>(req => req.Side == OrderSideStatus.BUY &&
                                           req.Symbol == "BTCUSDT" &&
                                           req.UsdtQuantity == 50 &&
                                           req.Laverage == 10),
                true),
                Times.Once
            );
        }
            
        [Fact]
        public async Task ReducePositionAsync_ShouldCreateOrderProcessWithSellSide()
        {
            // Arrange
            var robot = new TradeRobotInfo
            {
                Symbol = "BTCUSDT",
                HoldingQty = 0.01m,
                CurrentPositionCount = 1,
                CurrentPrice = 20000,
                Laverage = 5
            };

            var expectedResponse = new OrderResponse { Price = 20000, Quantity = 0.001m };
            _mockClient
                .Setup(c => c.CreateOrderProcessAsync(It.IsAny<OrderRequest>(), true))
                .ReturnsAsync(expectedResponse);

            // Act
            var actual = await _service.ReducePositionAsync(robot);

            // Assert
            Assert.Equal(expectedResponse, actual);

            _mockClient.Verify(client => client.CreateOrderProcessAsync(
                It.Is<OrderRequest>(req => req.Side == OrderSideStatus.SELL &&
                                           req.Symbol == "BTCUSDT" &&
                                           req.ReduceQty == 0.01m &&
                                           req.Laverage == 5),
                true),
                Times.Once
            );

        }
    }
}
