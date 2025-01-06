namespace GridBotStrategy.Observers
{
    internal interface IMarketDataObserver
    {
        void OnMarketDataReceived(string message);
    }
}
