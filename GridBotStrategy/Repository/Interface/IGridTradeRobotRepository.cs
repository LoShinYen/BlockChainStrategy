namespace GridBotStrategy.Repository.Interface
{
    public interface IGridTradeRobotRepository
    {

        Task CreateRobotAsync(GridTradeRobot robot);

        Task<List<GridTradeRobot>> GetAllRobotsAsync();

        Task<List<GridTradeRobot>> GetRunningRobotsAsync();

        Task DeleteRobotAsync(int robotId);

        void UpdateRobot(GridTradeRobot robot);

        void UpdateRobotByOrderProccssed(TradeRobotInfo robot);

        Task UpdateAPIKeyAsync(string encryptedApiKey, string encryptedApiSecret);

    }
}
