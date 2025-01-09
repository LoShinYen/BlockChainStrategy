
namespace GridBotStrategy.Repository
{
    internal class GridTradeDetailRepository : IGridTradeDetailRepository
    {
        private readonly CryptoPlatformDbContext _context;

        public GridTradeDetailRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public async Task CreateRobotDetailAsync(GridTradeRobotDetail detail)
        {
            await _context.GridTradeRobotDetails.AddAsync(detail);
            await _context.SaveChangesAsync();
        }
    }
}
