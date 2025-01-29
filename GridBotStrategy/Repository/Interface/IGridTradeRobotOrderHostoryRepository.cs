namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeRobotOrderHostoryRepository
    {
        Task CreateRobotOrderHistoryAsync(GridTradeRobotOrderHistory orderHistory);
    }
}
