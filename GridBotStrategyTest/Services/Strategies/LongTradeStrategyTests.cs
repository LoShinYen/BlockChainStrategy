using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Exchange.Interface;
using BlockChainStrategy.Library.Helpers.Interface;
using BlockChainStrategy.Library.Models.Dto.Utility;
using GridBotStrategy.Models;
using GridBotStrategy.Services.Strategies;
using Moq;

namespace GridBotStrategyTest.Services.Strategies
{
    public class LongTradeStrategyTests
    {
        // Step 1: 測試類別中的欄位
        private readonly Mock<IExchangeClientFactory> _mockFactory;
        private readonly Mock<IExchangeClient> _mockClient;
        private readonly LongTradeStrategy _strategy;

        public LongTradeStrategyTests()
        {
            // Step 2: 建構子中建立 Mock 物件與 SUT (System Under Test)

            // 2a. 建立工廠的 mock
            _mockFactory = new Mock<IExchangeClientFactory>();
            // 2b. 建立交易所客戶端的 mock
            _mockClient = new Mock<IExchangeClient>();

            // 如果策略會呼叫 ListenWebSocketAsync
            _mockClient.Setup(c => c.ListenWebSocketAsync()).Returns(Task.CompletedTask);

            // 當 factory 被呼叫時，就回傳 _mockClient.Object
            _mockFactory
                .Setup(f => f.GetExchangeClient(It.IsAny<ExchangeConfig>()))
                .Returns(_mockClient.Object);

            // 2c. 建立要測試的類別 (LongTradeStrategy)，並注入 mockFactory
            _strategy = new LongTradeStrategy(_mockFactory.Object);
        }

        #region ExecuteTradeAsync Tests
        [Fact]
        public async Task ExecuteTradeAsync_CheckIsOpenTrue_ShouldCallRaisePosition()
        {
            // Arrange
            var robot = new TradeRobotInfo { CurrentPositionCount = 0, LastPrice = 100, CurrentPrice = 100 };

            var mockResponse = new OrderResponse { Price = 20000, Quantity = 0.001m };
            _mockClient.Setup(c => c.CreateOrderProcessAsync(It.IsAny<OrderRequest>(), true))
                       .ReturnsAsync(mockResponse);

            // Act
            var actual = await _strategy.ExecuteTradeAsync(robot);

            // Assert
            // 檢查回傳結果
            Assert.Equal(mockResponse, actual);
            // 驗證：RaisePositionAsync => BUY
            _mockClient.Verify(c => c.CreateOrderProcessAsync(
                It.Is<OrderRequest>(req => req.Side == OrderSideStatus.BUY),
                true),
                Times.Once
            );
        }

        [Fact]
        public async Task ExecuteTradeAsync_CheckIsOpenFalse_PriceIsRaise_ShouldCallReducePosition()
        {
            // Arrange
            var robot = new TradeRobotInfo { CurrentPositionCount = 1, LastPrice = 100, CurrentPrice = 101 };

            var mockResponse = new OrderResponse { Price = 20000, Quantity = 0.001m };
            _mockClient
                .Setup(c => c.CreateOrderProcessAsync(It.IsAny<OrderRequest>(), true))
                .ReturnsAsync(mockResponse);

            // Act
            var actual = await _strategy.ExecuteTradeAsync(robot);

            // Assert
            Assert.Equal(mockResponse, actual);
            // 減倉 => SELL
            _mockClient.Verify(c => c.CreateOrderProcessAsync(
                It.Is<OrderRequest>(req => req.Side == OrderSideStatus.SELL),
                true),
                Times.Once
            );
        }

        [Fact]
        public async Task ExecuteTradeAsync_CheckIsOpenFalse_PriceIsNotRaise_ShouldCallRaisePosition()
        {
            // Arrange
            // PriceIsNotRaise => (LastPrice=101, CurrentPrice=100)
            var robot = new TradeRobotInfo { CurrentPositionCount = 1, LastPrice = 101, CurrentPrice = 100 };

            var mockResponse = new OrderResponse { Price = 20000, Quantity = 0.002m };
            _mockClient
                .Setup(c => c.CreateOrderProcessAsync(It.IsAny<OrderRequest>(), true))
                .ReturnsAsync(mockResponse);

            // Act
            var actual = await _strategy.ExecuteTradeAsync(robot);

            // Assert
            Assert.Equal(mockResponse, actual);
            // 還是 BUY
            _mockClient.Verify(c => c.CreateOrderProcessAsync(
                It.Is<OrderRequest>(req => req.Side == OrderSideStatus.BUY),
                true),
                Times.Once
            );
        }
        #endregion

        #region UpdatePositionInfo Tests
        [Fact]
        public void UpdatePositionInfo_BuyOrder_ShouldRecalcHoldingAndAvgPrice()
        {
            var robot = new TradeRobotInfo
            {
                HoldingQty = 1m,
                AvgHoldingPrice = 100m,
                CurrentPositionCount = 1,
                Postions = new List<TradeRobotPosition> 
                {
                    new TradeRobotPosition { IsLastTarget=false },
                    new TradeRobotPosition { IsLastTarget=false }
                },
                TargetPositionIndex = 1
            };
            var order = new OrderResponse
            {
                OrderSideStatus = OrderSideStatus.BUY,
                Quantity = 2m,
                Price = 120m
            };

            decimal expectedAvg = (1m * 100m + 2m * 120m) / (1m + 2m); // => 113.3333
            // Act
            _strategy.UpdatePositionInfo(order, robot);

            // Assert
            Assert.Equal(expectedAvg, robot.AvgHoldingPrice, precision: 4);
            Assert.Equal(3m, robot.HoldingQty);
            // 檢查 positions
            Assert.False(robot.Postions[0].IsLastTarget);
            Assert.True(robot.Postions[1].IsLastTarget);
        }

        [Fact]
        public void UpdatePositionInfo_SellOrder_CurPosCountIsZero_ShouldResetAvgPrice()
        {
            // Arrange
            var robot = new TradeRobotInfo
            {
                HoldingQty = 5m,
                AvgHoldingPrice = 100m,
                CurrentPositionCount = 0,
                Postions = new List<TradeRobotPosition> 
                {
                    new TradeRobotPosition { IsLastTarget=false },
                    new TradeRobotPosition { IsLastTarget=false }
                },
                TargetPositionIndex = 1
            };
            var order = new OrderResponse
            {
                OrderSideStatus = OrderSideStatus.SELL,
                Quantity = 5m,
                Price = 120m
            };

            // Act
            _strategy.UpdatePositionInfo(order, robot);

            // Assert
            // 若 CurrentPositionCount=0 => 直接 AvgHoldingPrice=0
            Assert.Equal(0m, robot.AvgHoldingPrice);
            Assert.Equal(0m, robot.HoldingQty);
            Assert.False(robot.Postions[0].IsLastTarget);
            Assert.True(robot.Postions[1].IsLastTarget);
        }

        [Fact]
        public void UpdatePositionInfo_SellOrder_CurPosCountIsNotZero_ShouldRecalcAvgPrice()
        {
            // Arrange
            var robot = new TradeRobotInfo
            {
                HoldingQty = 5m,
                AvgHoldingPrice = 100m,
                CurrentPositionCount = 1,
                Postions = new List<TradeRobotPosition> 
                { 
                    new TradeRobotPosition { IsLastTarget = false }, new TradeRobotPosition { IsLastTarget = false } 
                },
                TargetPositionIndex = 1
            };
            var order = new OrderResponse
            {
                OrderSideStatus = OrderSideStatus.SELL,
                Quantity = 2m,
                Price = 120m
            };

            var expected = (5m * 100m - 2m * 120m) / (5m - 2m); // => 86.6667

            // Act
            _strategy.UpdatePositionInfo(order, robot);

            // Assert
            Assert.Equal(expected, robot.AvgHoldingPrice, precision: 4);
            Assert.Equal(3m, robot.HoldingQty); // 5 - 2
            Assert.False(robot.Postions[0].IsLastTarget);
            Assert.True(robot.Postions[1].IsLastTarget);
        }
        #endregion
    }
}
