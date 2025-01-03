namespace GridBotStrategy.Services
{
    internal class RobotManagerService : IRobotManagerService
    {
        private readonly IGridTradeRobotRepository _gridTradeRobotRepository;

        public RobotManagerService(IGridTradeRobotRepository gridTradeRobotRepository)
        {
            _gridTradeRobotRepository = gridTradeRobotRepository;
        }

        public async Task ExcuteAsync()
        {
            var operation = RobotManagerHelper.AskForUserOperateRobotInfo();

            switch (operation)
            {
                case RobotOperation.CreateRobotInfo:
                    Console.WriteLine("執行創建機器人資訊...");

                    break;

                case RobotOperation.UpdateRobotParameters:
                    Console.WriteLine("執行更新機器人參數資訊...");

                    break;

                case RobotOperation.DeleteRobotInfo:
                    Console.WriteLine("執行刪除機器人資訊...");

                    break;

                case RobotOperation.ViewRobotInfo:
                    Console.WriteLine("執行查看機器人資訊...");

                    break;

                case RobotOperation.UpdateAllApiKeys:
                    Console.WriteLine("執行更新所有機器人 API Key...");
                    var (encryptedApiKey, encryptedApiSecret) = RobotManagerHelper.UpdateApiKeys();

                    break;

                case RobotOperation.RunDirectly:
                    Console.WriteLine("執行直接運行...");

                    break;

                case RobotOperation.Exit:
                    Console.WriteLine("退出程式...");
                    return;

                default:
                    Console.WriteLine("無效操作！");
                    break;
            }
        }

    }
}
