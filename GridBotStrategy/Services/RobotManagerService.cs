using Newtonsoft.Json;

namespace GridBotStrategy.Services
{
    internal class RobotManagerService : IRobotManagerService
    {
        private readonly IGridTradeRobotRepository _gridRobotRepository;
        private readonly IGridTradeRobotDetailRepository _gridTradeDetailRepository;

        public RobotManagerService(IGridTradeRobotRepository gridTradeRobotRepository, IGridTradeRobotDetailRepository gridTradeDetailRepository)
        {
            _gridRobotRepository = gridTradeRobotRepository;
            _gridTradeDetailRepository = gridTradeDetailRepository;
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
                        LoggerHelper.LogAndShowInfo("開始執行交易策略！");
                        break;
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    LoggerHelper.LogAndShowError($"資料庫操作失敗: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception e)
                {
                    LoggerHelper.LogAndShowError($"執行失敗，錯誤訊息：{e.Message}");
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
                    await UpdateRobotParamsInfoAsync();
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
                    LoggerHelper.LogAndShowInfo("執行直接運行...");
                    return true;

                default:
                    LoggerHelper.LogAndShowInfo("無效操作，請重新選擇！");
                    break;
            }

            return false;
        }

        #region Create Robot
        private async Task CreateRobotAsync()
        {
            LoggerHelper.LogAndShowInfo("執行創建機器人資訊...");

            var symbol = RobotManagerHelper.GetValidatedInput("請輸入交易貨幣(ex:BTCUSDT):");

            var positionSide = GetValidatedPositionSide();

            var (minPrice,maxPrice) = GetValidatedPriceRange();

            var gridCount = GetValidatedPositiveInteger("請輸入網格數量(正整數):", "網格數量必須大於 0，請重新輸入！");

            var laverage = GetValidatedPositiveInteger("請輸入槓桿倍數(可選，輸入 0 表示默認1倍):", "槓桿倍數不能為負，請重新輸入！");

            var amount = GetValidatedPositiveInteger("請輸入總金額交易金額(USDT正整數):", "交易金額必須大於 0，請重新輸入！");

            var (encryptedApiKey, encryptedApiSecret) = RobotManagerHelper.EncryptApiKeys();

            var robot = new GridTradeRobot
            {
                Symbol = symbol,
                StatusEnum = GridTradeRobotStatus.Open,
                AmountUsdt = amount,
                PositionSide = positionSide,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                GridCount = gridCount,
                Laverage = laverage,
                EncryptedApiKey = encryptedApiKey,
                EncryptedApiSecret = encryptedApiSecret
            };

            await CreateRobotInfoAsync(robot);
            LoggerHelper.LogAndShowInfo("機器人創建成功！");
        }

        private async Task CreateRobotInfoAsync(GridTradeRobot robot)
        {
            await _gridRobotRepository.CreateRobotAsync(robot);
            var positions = CalcutePositionInfo(robot);
            var robotDetails = new GridTradeRobotDetail
            {
                GridTradeRobotId = robot.GridTradeRobotId,
                GridInfos = JsonConvert.SerializeObject(positions),
            };

            await _gridTradeDetailRepository.CreateRobotDetailAsync(robotDetails);
        }

        private List<TradeRobotPosition> CalcutePositionInfo(GridTradeRobot robot)
        {
            var positions = new List<TradeRobotPosition>();

            decimal gridAmount = (robot.MaxPrice - robot.MinPrice) / robot.GridCount;

            for (int i = 0; i < robot.GridCount; i++)
            {
                var position = new TradeRobotPosition
                {
                    TargetIndex = i,
                    TargetPrice = robot.MinPrice + gridAmount * i,
                    IsActivated = false,
                    IsLastTarget = false
                };
                positions.Add(position);
            }

            return positions;
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

        #region Update Robot Parameters

        private async Task UpdateRobotParamsInfoAsync()
        {
            LoggerHelper.LogAndShowInfo("執行更新機器人參數資訊...");
            var robots = await _gridRobotRepository.GetAllRobotsAsync();

            if (robots.Count == 0)
            {
                Console.WriteLine("沒有機器人資訊！");
                return;
            }

            ShowRobotInfo(robots);
            var inputRobotId = CkeckInptRobotIdValidated(robots, "請輸入要更新的機器人編號：");

            ShowSelectParams();

            var inputParam = ValidateUpdateParamInput();

            var selectRobot = robots.First(r => r.GridTradeRobotId == inputRobotId);

            UpdateParams(selectRobot, inputParam);
        }

        private static void ShowSelectParams()
        {
            Console.WriteLine("請輸入要更新的參數：");
            foreach (var param in Enum.GetValues(typeof(UpdateRobotParams)))
            {
                Console.WriteLine($"{(int)param}: {param}");
            }
        }

        private UpdateRobotParams ValidateUpdateParamInput()
        {
            while (true)
            {
                var input = RobotManagerHelper.GetValidatedIntInput("請輸入參數編號：");
                if (Enum.IsDefined(typeof(UpdateRobotParams), input))
                {
                    return (UpdateRobotParams)input;
                }
                Console.WriteLine("無效的參數選擇，請重新輸入！");
            }
        }

        private void UpdateParams(GridTradeRobot robot, UpdateRobotParams inputParam)
        {
            switch (inputParam)
            {
                case UpdateRobotParams.Status:
                    var newStatus = RobotManagerHelper.GetValidatedInput("請輸入新的狀態（Open/Cancel）：");
                    robot.StatusEnum = inputParam.ToString().Equals("Open", StringComparison.OrdinalIgnoreCase) ? GridTradeRobotStatus.Open : GridTradeRobotStatus.Cancel;
                    break;

                case UpdateRobotParams.Symbol:
                    var newSymbol = RobotManagerHelper.GetValidatedInput("請輸入新的交易貨幣（如：BTCUSDT）：");
                    robot.Symbol = newSymbol;
                    break;

                case UpdateRobotParams.AmountUsdt:
                    var newAmount = GetValidatedPositiveInteger("請輸入新的總金額：", "總金額必須大於 0，請重新輸入！");
                    robot.AmountUsdt = newAmount;
                    break;

                case UpdateRobotParams.PositionSide:
                    var newPositionSide = GetValidatedPositionSide();
                    robot.PositionSide = newPositionSide;
                    break;

                case UpdateRobotParams.GridCount:
                    var newGridCount = GetValidatedPositiveInteger("請輸入新的網格數量：", "網格數量必須大於 0，請重新輸入！");
                    robot.GridCount = newGridCount;
                    break;

                case UpdateRobotParams.Leverage:
                    var newLaverage = GetValidatedPositiveInteger("請輸入新的槓桿倍數：", "槓桿倍數不能為負，請重新輸入！");
                    robot.Laverage = newLaverage;
                    break;

                case UpdateRobotParams.PriceRange:
                    var (newMinPrice, newMaxPrice) = GetValidatedPriceRange();
                    robot.MinPrice = newMinPrice;
                    robot.MaxPrice = newMaxPrice;
                    break;

                case UpdateRobotParams.ApiKey:
                    var (newApiKey, newApiSecret) = RobotManagerHelper.EncryptApiKeys();
                    robot.EncryptedApiKey = newApiKey;
                    robot.EncryptedApiSecret = newApiSecret;
                    break;

                default:
                    LoggerHelper.LogAndShowInfo("選擇的參數無效！");
                    break;
            }

            robot.UpdatedAt = DateTime.UtcNow;
            _gridRobotRepository.UpdateRobot(robot);
        }

        #endregion

        #region Delete Robot

        private async Task DeleteRobotAsync()
        {
            LoggerHelper.LogAndShowInfo("執行刪除機器人資訊...");
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
                Console.WriteLine($"交易貨幣：{robot.Symbol} , 槓桿倍數 : {robot.Laverage} , 網格金額 : {robot.MaxPrice} ~ {robot.MinPrice} ");
            }

            int deleteRobotId = CkeckInptRobotIdValidated(robots, "請輸入要刪除的機器人編號：");

            await _gridRobotRepository.DeleteRobotAsync(deleteRobotId);
            LoggerHelper.LogAndShowInfo($"機器人 RobotID : {deleteRobotId} 刪除成功！");
        }

        #endregion

        #region View Robot Info

        private async Task ViewRobotInfoAsync()
        {
            LoggerHelper.LogAndShowInfo("執行查看機器人資訊...");
            var robots = await _gridRobotRepository.GetAllRobotsAsync();
            if (robots.Count == 0)
            {
                Console.WriteLine("沒有機器人資訊！");
                return;
            }
            ShowRobotInfo(robots);
            LoggerHelper.LogAndShowInfo("查看完畢");
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

        private int CkeckInptRobotIdValidated(List<GridTradeRobot> robots, string prompt)
        {
            while (true)
            {
                int deleteRobotId = RobotManagerHelper.GetValidatedIntInput(prompt);
                if (robots.Any(r => r.GridTradeRobotId == deleteRobotId))
                {
                    return deleteRobotId;
                }
                Console.WriteLine("輸入無效，請重新輸入！");
            }
        }

        private static void ShowRobotInfo(List<GridTradeRobot> robots)
        {
            Console.WriteLine("所有機器人資訊：");

            foreach (var robot in robots)
            {
                var apiKey = EncryptionHelper.Decrypt(robot.EncryptedApiKey);
                var apiSecret = EncryptionHelper.Decrypt(robot.EncryptedApiSecret);
                Console.WriteLine($"【RobotID : {robot.GridTradeRobotId}】 詳細資訊 :");
                Console.WriteLine(
                    $"交易貨幣：{robot.Symbol},機器人狀態 : {robot.Status},總金額(USDT) : {robot.AmountUsdt} ,持倉方向 : {robot.PositionSide} , 槓桿倍數 : {robot.Laverage} , " +
                    $"網格金額 : {robot.MaxPrice} ~ {robot.MinPrice} , 網格數量 : {robot.GridCount} ," +
                    $"API Key : {apiKey} , API Secret : {apiSecret}"
                );
            }
        }

    }
}
