namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeRobotRepository
    {
        Task CreateRobotAsync(GridTradeRobot robot);

        Task  UpdateAPIKeyAsync(string encryptedApiKey, string encryptedApiSecret);
    }
}
