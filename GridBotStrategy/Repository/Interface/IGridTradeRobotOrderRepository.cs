namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeRobotOrderRepository
    {
        Task CreateRobotOrderAsync(GridTradeRobotOrder order);

        void UpdateRobotOrder(GridTradeRobotOrder order);

        Task<GridTradeRobotOrder?> GetRunningOrderByRobotIdAsync(int robotId);
    }
}
