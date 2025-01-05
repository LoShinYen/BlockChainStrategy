namespace GridBotStrategy.Repository
{
    internal class GridTradeRobotRepository : IGridTradeRobotRepository
    { 
        private readonly CryptoPlatformDbContext _context;

        public GridTradeRobotRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public async Task CreateRobotAsync(GridTradeRobot robot)
        {
            await _context.GridTradeRobots.AddAsync(robot);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GridTradeRobot>> GetAllRobotsAsync()
        { 
            var robots = await _context.GridTradeRobots.ToListAsync();
            return robots;
        }

        public async Task DeleteRobotAsync(int robotId)
        {
            var robot = await _context.GridTradeRobots.FindAsync(robotId);
            if (robot != null)
            {
                _context.GridTradeRobots.Remove(robot);
                await _context.SaveChangesAsync();
            }
        }

        public void UpdateRobot(GridTradeRobot robot)
        {
            _context.GridTradeRobots.Update(robot);
            _context.SaveChanges();
        }

        public async Task UpdateAPIKeyAsync(string encryptedApiKey, string encryptedApiSecret)
        {
            await _context.GridTradeRobots
                .ExecuteUpdateAsync(robot => robot
                    .SetProperty(r => r.EncryptedApiKey, encryptedApiKey)
                    .SetProperty(r => r.EncryptedApiSecret, encryptedApiSecret));
        }
    }
}
