namespace GridBotStrategy.Services
{
    public class BaseStratgyService
    {
        public bool CheckIsOpen(int currentPositionCount)
        {
            if (currentPositionCount > 0) return true;
            return false;
        }

        public async Task OpenPositionAsync()
        {
            // Open Position
        }


        public async Task RaisePositionAsync()
        {
            // Raise Position
        }

        public async Task ClosePositionAsync()
        {
            // Close Position
        }

        public bool CheckPriceIsRaise(decimal lastPrice, decimal currentPrice)
        {
            return lastPrice < currentPrice;
        }

    }
}
