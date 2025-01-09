namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeDetailRepository
    {
        Task CreateRobotDetailAsync(GridTradeRobotDetail detail);
    }
}
