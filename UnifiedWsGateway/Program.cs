using Microsoft.Extensions.Configuration;
using UnifiedWsGateway.Services;

class Proram
{
    static async Task Main(string[] args)
    {
        try
        { 
            LoggerHelper.LogInfo("UnifiedWsGateway is Start");
            var configuration = BuildConfiguration();
            var symbols = FetchMarketPricSymbol(configuration);
            var tasks = new List<Task>();
            var binanceMarketPriceService = new BinanceMarketPriceService(symbols!);
            tasks.Add(Task.Run(async () => await binanceMarketPriceService.StartWebSocketService()));

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(ex.Message);
        }
        finally
        {
            LoggerHelper.LogInfo("UnifiedWsGateway is Stop");
        }
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var builder = new ConfigurationBuilder()
           .SetBasePath(basePath)
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        return builder.Build();
    }

    private static List<string> FetchMarketPricSymbol(IConfigurationRoot configuration)
    { 
        var symbols = configuration.GetSection("BinanceWebsocket:MarketPriceSymbol").Get<List<string>>();
        if(symbols == null || symbols.Count() == 0) throw new Exception("Appsetting MarketPriceSymbol is empty");
        return symbols;
    }

}