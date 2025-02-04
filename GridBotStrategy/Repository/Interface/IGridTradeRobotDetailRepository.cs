namespace GridBotStrategy.Repository.Interface
{
    public interface IGridTradeRobotDetailRepository
    {
        Task<GridTradeRobotDetail?> GetDetailByRobotIdAsync(int robotId);

        Task CreateRobotDetailAsync(GridTradeRobotDetail detail);

        void UpdateRobotDetail(GridTradeRobotDetail detail);
    }
}
