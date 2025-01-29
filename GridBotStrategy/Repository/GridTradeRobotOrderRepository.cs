namespace GridBotStrategy.Repository
{
    internal class GridTradeRobotOrderRepository : IGridTradeRobotOrderRepository
    {
        private readonly CryptoPlatformDbContext _context;

        public GridTradeRobotOrderRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public async Task CreateRobotOrderAsync(GridTradeRobotOrder order)
        {
            await _context.GridTradeRobotOrders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public void UpdateRobotOrder(GridTradeRobotOrder order)
        {
            _context.GridTradeRobotOrders.Update(order);
        }

        public async Task<GridTradeRobotOrder?> GetRunningOrderByRobotIdAsync(int robotId)
        {
            return await _context.GridTradeRobotOrders.FirstOrDefaultAsync(o => o.GridTradeRobotId == robotId && o.Status =="Running");
        }

    }
}
