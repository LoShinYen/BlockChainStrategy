namespace GridBotStrategy.Repository
{
    internal class GridTradeRobotOrderHostoryRepository : IGridTradeRobotOrderHostoryRepository
    {
        private readonly CryptoPlatformDbContext _context;

        public GridTradeRobotOrderHostoryRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public async Task CreateRobotOrderHistoryAsync(GridTradeRobotOrderHistory orderHistory)
        {
            await _context.GridTradeRobotOrderHistories.AddAsync(orderHistory);
        }

    }
}
