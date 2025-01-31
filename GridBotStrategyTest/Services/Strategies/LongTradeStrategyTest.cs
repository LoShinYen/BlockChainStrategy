using BlockChainStrategy.Library.Helpers.Interface;
using GridBotStrategy.Services.Strategies;
using Moq;

namespace GridBotStrategyTest.Services.Strategies
{
    public class LongTradeStrategyTest
    {
        private readonly Mock<IExchangeClientFactory> _mockFactory;
        private readonly LongTradeStrategy _service;

        public LongTradeStrategyTest(Mock<IExchangeClientFactory> mockFactory)
        {
            _mockFactory = mockFactory;
            _service = new LongTradeStrategy(_mockFactory.Object);
        }



    }
}
