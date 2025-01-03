namespace GridBotStrategy.Repository
{
    internal class GridTradeRobotRepository : IGridTradeRobotRepository
    { 
        private readonly CryptoPlatformDbContext _context;

        public GridTradeRobotRepository(CryptoPlatformDbContext context)
        {
            _context = context;
        }

        public Task UpdateAsync(string encryptedApiKey, string encryptedApiSecret)
        {
            throw new NotImplementedException();
        }
    }
}
