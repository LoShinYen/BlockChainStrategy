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

        public async Task UpdateAPIKeyAsync(string encryptedApiKey, string encryptedApiSecret)
        {
            await _context.GridTradeRobots
                .ExecuteUpdateAsync(robot => robot
                    .SetProperty(r => r.EncryptedApiKey, encryptedApiKey)
                    .SetProperty(r => r.EncryptedApiSecret, encryptedApiSecret));
        }
    }
}
