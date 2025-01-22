using BlockChainStrategy.Library.Models.Dto;
using System.Security.Cryptography;
using System.Text;

namespace BlockChainStrategy.Library.Helpers
{
    public class BinanceHelper
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string _baseUrl = string.Empty;
        private string _apiKey = string.Empty;
        private string _apiSecert = string.Empty;

        public BinanceHelper(bool test = false)
        {
            SelectBaseUrl(test);
        }

        public BinanceHelper(string apiKey, string apiSecert, bool test = false)
        {
            SelectBaseUrl(test);
            _apiKey = apiKey;
            _apiSecert = apiSecert;
        }

        private void SelectBaseUrl(bool test)
        {
            _baseUrl = test ? "https://testnet.binancefuture.com/fapi/v1" : "https://fapi.binance.com/fapi/v1";
        }

        public async Task<BinanceCreateOrderResponseDto?> CreateOrderAsync(BinanceCreateOrderRequestDto request)
        {
            string endpoint = "/order";

            string url = $"{_baseUrl}{endpoint}";

            var parameters = new Dictionary<string, string>
            {
                { "symbol", request.Symbol },
                { "side", request.Side.ToString() },
                { "positionSide", request.PositionSide.ToString() },
                { "type", request.Type.ToString() },
                { "quantity", request.Quantity.ToString() },
                { "timestamp", GetTimestamp().ToString() }
            };

            string queryString = GetQueryString(parameters);
            string signature = GenerateSignature(queryString);

            parameters.Add("signature", signature);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error from Binance API: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BinanceCreateOrderResponseDto>(content);
            return result;
        }

        /// <summary>
        /// 切換持倉模式
        /// </summary>
        /// <param name="dualSidePosition">true 雙向持倉, false 單向持倉</param>
        /// <returns></returns>
        public async Task<BinanceChangePositionModeResponseDto?> ChangePositionModeAsync(bool dualSidePosition)
        {
            string endpoint = "/positionSide/dual";

            string url = $"{_baseUrl}{endpoint}";

            var parameters = new Dictionary<string, string>
            {   
                { "dualSidePosition", dualSidePosition.ToString().ToLower() },
                { "timestamp", GetTimestamp().ToString() }
            };

            string queryString = GetQueryString(parameters);
            string signature = GenerateSignature(queryString);

            parameters.Add("signature", signature);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error from Binance API: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BinanceChangePositionModeResponseDto>(content);
            return result;
        }

        public async Task<BinanceChangeLeverageResponseDto?> ChangeLeverageAsync(BinanceChangeLeverageRequestDto request)
        {
            string endpoint = "/leverage";

            string url = $"{_baseUrl}{endpoint}";

            var parameters = new Dictionary<string, string>
            {
                { "symbol", request.Symbol },
                { "leverage", request.Leverage.ToString() },
                { "timestamp", GetTimestamp().ToString() }
            };

            string queryString = GetQueryString(parameters);
            string signature = GenerateSignature(queryString);

            parameters.Add("signature", signature);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            httpRequest.Headers.Add("X-MBX-APIKEY", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error from Binance API: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BinanceChangeLeverageResponseDto>(content);
            return result;
        }

        private long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private string GetQueryString(Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    sb.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}&");
                }
            }
            return sb.ToString().TrimEnd('&');
        }

        private string GenerateSignature(string queryString)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecert));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

    }
}
