namespace GridBotStrategy.Helpers
{
    internal class RobotApiKeyManager
    {
        internal static void UpdateApiKeys()
        {
            Console.WriteLine("是否需要更新所有機器人 API Key? (Y/N)");
            string? input = Console.ReadLine();
            if (input?.Trim().ToUpper() == "Y")
            {
                string newApiKey = GetValidatedInput("請輸入新的 API Key:");
                string newApiSecret = GetValidatedInput("請輸入新的 API Secret:");

                try
                {
                    string encryptedApiKey = EncryptionHelper.Encrypt(newApiKey);
                    string encryptedApiSecret = EncryptionHelper.Encrypt(newApiSecret);

                    Console.WriteLine("加密成功！");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加密過程中發生錯誤：{ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("本次操作不更新API Key 等資訊。");
            }
        }

        /// <summary>
        /// 獲取有效輸入，並檢查是否為空值。
        /// </summary>
        /// <param name="prompt">提示訊息</param>
        /// <returns>用戶的有效輸入</returns>
        private static string GetValidatedInput(string prompt)
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
