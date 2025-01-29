using GridBotStrategy.Extensions;
using GridBotStrategy.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            LoggerHelper.LogInfo("程式開始");

            IHost host = BuildHost(args);
            await RunApplicationAsync(host);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError($"發生錯誤：{ex.Message}");
        }
        finally
        {
            LoggerHelper.LogInfo("程式結束");
        }
    }

    private static IHost BuildHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("CryptoPlatformDb");
                DIExtensions.AddServicesAndDependencies(services, connectionString);
            })
            .Build();
    }

    private static async Task RunApplicationAsync(IHost host)
    {
        //await RobotManageAsync(host);
        await RunningTradeAsync(host);
    }

    private static async Task RobotManageAsync(IHost host)
    {
        LoggerHelper.LogInfo("開始機器人管理操作...");
        var robotManagerService = host.Services.GetRequiredService<IRobotManagerService>();
        await robotManagerService.ExcuteAsync();
        LoggerHelper.LogInfo("機器人管理操作完成");
    }

    private static async Task RunningTradeAsync(IHost host)
    {
        LoggerHelper.LogInfo("開始執行交易策略...");
        var task = new List<Task>();
        var trade = host.Services.GetRequiredService<TradeExecutionService>();
        var ws = host.Services.GetRequiredService<MarketDataSubscriptionHandler>();
        ws.Subscribe(trade);

        //task.Add(Task.Run(async () => { await ws.ConnectAsync(); }));
        task.Add(Task.Run(async () => { await trade.ExcuteTradeAsync(); }));

        await Task.WhenAll(task);
        LoggerHelper.LogInfo("交易策略執行完成");
    }

}