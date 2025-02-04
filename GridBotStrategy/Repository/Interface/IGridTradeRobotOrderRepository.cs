namespace GridBotStrategy.Repository.Interface
{
    public interface IGridTradeRobotOrderRepository
    {
        Task CreateRobotOrderAsync(GridTradeRobotOrder order);

        void UpdateRobotOrder(GridTradeRobotOrder order);

        Task<GridTradeRobotOrder?> GetRunningOrderByRobotIdAsync(int robotId);
    }
}
