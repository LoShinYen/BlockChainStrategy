using GridBotStrategy.Extensions;
using GridBotStrategy.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
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
        
        var robotManagerService = host.Services.GetRequiredService<IRobotManagerService>();
        await robotManagerService.ExcuteAsync();

        var task = new List<Task>();

        var trade = host.Services.GetRequiredService<TradeExecutionService>();
        var ws = host.Services.GetRequiredService<MarketDataSubscriptionManager>();

        ws.Subscribe(trade);

        task.Add(Task.Run(async () => { await ws.ConnectAsync();}));
        task.Add(Task.Run(async () => { await trade.ExcuteTradeAsync();}));

        await Task.WhenAll(task);

        // 開發中使用
        Console.ReadLine();
    }
}