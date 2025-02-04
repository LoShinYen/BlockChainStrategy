namespace GridBotStrategy.Repository.Interface
{
    public interface IGridTradeRobotOrderHostoryRepository
    {
        Task CreateRobotOrderHistoryAsync(GridTradeRobotOrderHistory orderHistory);
    }
}
