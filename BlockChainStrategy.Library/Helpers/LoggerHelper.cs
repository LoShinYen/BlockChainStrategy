using NLog;

namespace BlockChainStrategy.Library.Helpers
{
    internal static class LoggerHelper
    {
        internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            Logger.Info(message);
            Console.WriteLine(message);
        }

        public static void LogError(string message)
        {
            Logger.Error(message);
            Console.WriteLine(message);
        }
    }
}
