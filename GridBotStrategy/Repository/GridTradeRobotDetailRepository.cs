namespace GridBotStrategy.Repository
{
    internal class GridTradeRobotDetailRepository : IGridTradeRobotDetailRepository
    {
        private readonly CryptoPlatformDbContext _context;

        public GridTradeRobotDetailRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<GridTradeRobotDetail?> GetDetailByRobotIdAsync(int robotId)
        {
            return await _context.GridTradeRobotDetails.FirstOrDefaultAsync(x => x.GridTradeRobotId == robotId);
        }

        public async Task CreateRobotDetailAsync(GridTradeRobotDetail detail)
        {
            await _context.GridTradeRobotDetails.AddAsync(detail);
            await _context.SaveChangesAsync();
        }

        public void UpdateRobotDetail(GridTradeRobotDetail detail)
        {
            _context.GridTradeRobotDetails.Update(detail);
        }
    }
}
