namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeRobotRepository
    {

        Task CreateRobotAsync(GridTradeRobot robot);

        Task<List<GridTradeRobot>> GetAllRobotsAsync();

        Task DeleteRobotAsync(int robotId);

        Task UpdateAPIKeyAsync(string encryptedApiKey, string encryptedApiSecret);
    }
}
