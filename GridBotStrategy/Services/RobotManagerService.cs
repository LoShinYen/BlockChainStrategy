﻿namespace GridBotStrategy.Services
{
    internal class RobotManagerService : IRobotManagerService
    {
        private readonly IGridTradeRobotRepository _gridRobotRepository;

        public RobotManagerService(IGridTradeRobotRepository gridTradeRobotRepository)
        {
            _gridRobotRepository = gridTradeRobotRepository;
        }

        public async Task ExcuteAsync()
        {
            while (true)
            {
                var operation = RobotManagerHelper.AskForUserOperateRobotInfo();
                try
                {
                    var isExit = await HandleOperationAsync(operation);
                    if (isExit)
                    {
                        LoggerHelper.LogInfo("開始執行交易策略！");
                        break;
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    LoggerHelper.LogError($"資料庫操作失敗: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception e)
                {
                    LoggerHelper.LogError($"執行失敗，錯誤訊息：{e.Message}");
                }
            }
        }

        private async Task<bool> HandleOperationAsync(RobotOperation operation)
        {
            switch (operation)
            {
                case RobotOperation.CreateRobotInfo:
                    await CreateRobotAsync();
                    break;

                case RobotOperation.UpdateRobotParameters:
                    LoggerHelper.LogInfo("執行更新機器人參數資訊...");
                    break;

                case RobotOperation.DeleteRobotInfo:
                    await DeleteRobotAsync();
                    break;

                case RobotOperation.ViewRobotInfo:
                    await ViewRobotInfoAsync();
                    break;

                case RobotOperation.UpdateAllApiKeys:
                    await UpdateRobotApiKeyInfoAsync();
                    break;

                case RobotOperation.RunDirectly:
                    LoggerHelper.LogInfo("執行直接運行...");
                    return true;

                default:
                    LoggerHelper.LogInfo("無效操作，請重新選擇！");
                    break;
            }

            return false;
        }

        #region Create Robot
        private async Task CreateRobotAsync()
        {
            LoggerHelper.LogInfo("執行創建機器人資訊...");

            var symbol = RobotManagerHelper.GetValidatedInput("請輸入交易貨幣(ex:BTCUSDT):");

            var positionSide = GetValidatedPositionSide();

            var (minPrice,maxPrice) = GetValidatedPriceRange();

            var gridCount = GetValidatedPositiveInteger("請輸入網格數量(正整數):", "網格數量必須大於 0，請重新輸入！");

            var laverage = GetValidatedPositiveInteger("請輸入槓桿倍數(可選，輸入 0 表示默認1倍):", "槓桿倍數不能為負，請重新輸入！");

            var (encryptedApiKey, encryptedApiSecret) = RobotManagerHelper.EncryptApiKeys();

            var robot = new GridTradeRobot
            {
                Symbol = symbol,
                StatusEnum = GridTradeRobotStatus.Open,
                PositionSide = positionSide,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                GridCount = gridCount,
                Leverage = laverage,
                EncryptedApiKey = encryptedApiKey,
                EncryptedApiSecret = encryptedApiSecret
            };

            await _gridRobotRepository.CreateRobotAsync(robot);
            LoggerHelper.LogInfo("機器人創建成功！");
        }

        private static string GetValidatedPositionSide()
        {
            while (true)
            {
                string inputPositionSide = RobotManagerHelper.GetValidatedInput("請輸入持倉方向(Long/Short/All):");
                if (
                    inputPositionSide.Equals(GridTradeRobotPositionSide.Long.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    inputPositionSide.Equals(GridTradeRobotPositionSide.Short.ToString(), StringComparison.OrdinalIgnoreCase) ||
                    inputPositionSide.Equals(GridTradeRobotPositionSide.All.ToString(), StringComparison.OrdinalIgnoreCase)
                    )
                {
                    return inputPositionSide;
                }
                Console.WriteLine("輸入無效，請重新輸入！");
            }
        }

        private int GetValidatedPositiveInteger(string prompt , string errorMsg)
        {
            while (true)
            {
                int result = RobotManagerHelper.GetValidatedIntInput(prompt);
                if (result > 0)
                    return result;

                Console.WriteLine(errorMsg);
            }
        }

        private (decimal,decimal) GetValidatedPriceRange()
        {
            while (true)
            {
                decimal minPrice = RobotManagerHelper.GetValidatedDecimalInput("請輸入金額下限:");
                decimal maxPrice = RobotManagerHelper.GetValidatedDecimalInput("請輸入金額上限:");
                if (minPrice >= maxPrice)
                {
                    Console.WriteLine("金額下限必須小於金額上限，請重新輸入！");
                }
                else
                { 
                    return (minPrice, maxPrice);
                }
            }
        }

        #endregion

        #region Delete Robot

        private async Task DeleteRobotAsync()
        {
            LoggerHelper.LogInfo("執行刪除機器人資訊...");
            var robots = await _gridRobotRepository.GetAllRobotsAsync();
            if (robots.Count == 0)
            {
                Console.WriteLine("沒有機器人資訊！");
                return;
            }

            Console.WriteLine("請選擇要刪除的機器人：");
            foreach (var robot in robots)
            {
                Console.WriteLine($"【RobotID : {robot.GridTradeRobotId}】 詳細資訊 :");
                Console.WriteLine($"交易貨幣：{robot.Symbol} , 槓桿倍數 : {robot.Leverage} , 網格金額 : {robot.MaxPrice} ~ {robot.MinPrice} ");
            }

            int deleteRobotId = CkeckInptDeleteRobotId(robots);

            await _gridRobotRepository.DeleteRobotAsync(deleteRobotId);
            LoggerHelper.LogInfo($"機器人 RobotID : {deleteRobotId} 刪除成功！");
        }

        private int CkeckInptDeleteRobotId(List<GridTradeRobot> robots)
        {
            while (true)
            {
                int deleteRobotId = RobotManagerHelper.GetValidatedIntInput("請輸入要刪除的機器人編號：");
                if (robots.Any(r => r.GridTradeRobotId == deleteRobotId))
                {
                    return deleteRobotId;
                }
                Console.WriteLine("輸入無效，請重新輸入！");
            }
        }

        #endregion

        #region View Robot Info

        private async Task ViewRobotInfoAsync()
        {
            LoggerHelper.LogInfo("執行查看機器人資訊...");
            var robots = await _gridRobotRepository.GetAllRobotsAsync();
            if (robots.Count == 0)
            {
                Console.WriteLine("沒有機器人資訊！");
                return;
            }

            Console.WriteLine("所有機器人資訊：");
            foreach (var robot in robots)
            {
                var apiKey = EncryptionHelper.Decrypt(robot.EncryptedApiKey);
                var apiSecret = EncryptionHelper.Decrypt(robot.EncryptedApiSecret);
                Console.WriteLine($"【RobotID : {robot.GridTradeRobotId}】 詳細資訊 :");
                Console.WriteLine(
                    $"交易貨幣：{robot.Symbol},機器人狀態 : {robot.Status}, 持倉方向 : {robot.PositionSide} , 槓桿倍數 : {robot.Leverage} , " +
                    $"網格金額 : {robot.MaxPrice} ~ {robot.MinPrice} , 網格數量 : {robot.GridCount} ,"+
                    $"API Key : {apiKey} , API Secret : {apiSecret}"
                );
            }
            LoggerHelper.LogInfo("查看完畢");
        }

        #endregion

        #region Update Robot API Key

        private async Task UpdateRobotApiKeyInfoAsync()
        {
            Console.WriteLine("執行更新所有機器人 API Key...");
            var (encryptedApiKey, encryptedApiSecret) = RobotManagerHelper.EncryptApiKeys();
            await _gridRobotRepository.UpdateAPIKeyAsync(encryptedApiKey, encryptedApiSecret);
        }

        #endregion
    }
}
