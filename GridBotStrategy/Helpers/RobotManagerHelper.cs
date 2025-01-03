using GridBotStrategy.Enums.GridRobot;

namespace GridBotStrategy.Helpers
{
    internal class RobotManagerHelper
    {

        internal static RobotOperation AskForUserOperateRobotInfo()
        {
            Console.WriteLine("請問使用者想要執行何種操作:");
            Console.WriteLine("1. 創建機器人資訊");
            Console.WriteLine("2. 更新機器人參數資訊");
            Console.WriteLine("3. 刪除機器人資訊");
            Console.WriteLine("4. 查看機器人資訊");
            Console.WriteLine("5. 更新所有機器人 API Key");
            Console.WriteLine("6. 直接運行");
            Console.WriteLine("7. 退出");

            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int choice) && Enum.IsDefined(typeof(RobotOperation), choice))
                {
                    return (RobotOperation)choice;
                }

                Console.WriteLine("無效的選項，請重新輸入有效的操作編號 (1-7):");
            }

        }

        /// <summary>
        /// 處理 API Key 和 Secret 的加密及更新。
        /// </summary>
        ///<returns>加密後的API Key等資訊</returns>
        internal static (string, string) EncryptApiKeys()
        {
            string newApiKey = GetValidatedInput("請輸入新的 API Key:");
            string newApiSecret = GetValidatedInput("請輸入新的 API Secret:");

            try
            {
                string encryptedApiKey = EncryptionHelper.Encrypt(newApiKey);
                string encryptedApiSecret = EncryptionHelper.Encrypt(newApiSecret);

                Console.WriteLine("加密成功！");
                LoggerHelper.LogInfo($"本次更新成功,Encrypt API Key :{encryptedApiKey} , Encrypt API Secret : {encryptedApiSecret}");
                return (encryptedApiKey, encryptedApiSecret);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError($"加密過程中發生錯誤：{ex.Message}");
                throw new Exception($"加密過程中發生錯誤：{ex.Message}");
            }
        }

        /// <summary>
        /// 檢查整數輸入是否有效。
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>回傳整數</returns>
        internal static int GetValidatedIntInput(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                Console.WriteLine("輸入無效，請輸入有效的整數！");
            }
        }

        /// <summary>
        /// 檢查Decimal輸入是否有效。
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>回傳Decimal</returns>
        internal static decimal GetValidatedDecimalInput(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string? input = Console.ReadLine();
                if (decimal.TryParse(input, out decimal result) && result > 0)
                {
                    return result;
                }
                Console.WriteLine("輸入無效，請輸入有效的正數！");
            }
        }


        /// <summary>
        /// 獲取有效輸入，並檢查是否為空值。
        /// </summary>
        /// <param name="prompt">提示訊息</param>
        /// <returns>用戶的有效輸入</returns>
        internal static string GetValidatedInput(string prompt)
        {
            string? input;
            while (true)
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }
                Console.WriteLine("輸入無效，請重新輸入！");
            }
        }
    
    }
}
