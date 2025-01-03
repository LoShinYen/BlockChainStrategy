namespace GridBotStrategy.Repository.Interface
{
    internal interface IGridTradeRobotRepository
    {
        Task  UpdateAsync(string encryptedApiKey, string encryptedApiSecret);
    }
}
