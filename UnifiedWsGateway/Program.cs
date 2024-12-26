using Microsoft.Extensions.Configuration;
using System.Net;
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
            var internalSubscriptionPublisherService = new InternalSubscriptionPublisherService();

            internalSubscriptionPublisherService.SubscribeMarketData(binanceMarketPriceService);

            tasks.Add(Task.Run(async () => await binanceMarketPriceService.StartWebSocketService()));

            tasks.Add(Task.Run(async () => await StartWebSocketServer(internalSubscriptionPublisherService)));

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


    private static async Task StartWebSocketServer(InternalSubscriptionPublisherService internalSubscriptionPublisherService)
    {
        HttpListener httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:5001/subscribe/");
        httpListener.Start();
        LoggerHelper.LogInfo("WebSocket server started on ws://localhost:5001/subscribe/");

        while (true)
        {
            var httpListenerContext = await httpListener.GetContextAsync();

            if (httpListenerContext.Request.IsWebSocketRequest)
            {
                var remoteEndPoint = httpListenerContext.Request.RemoteEndPoint;
                var webSocketContext = await httpListenerContext.AcceptWebSocketAsync(null);

                _ = Task.Run(async () =>
                {
                    await internalSubscriptionPublisherService.HandleClientWebSocketAsync(webSocketContext.WebSocket, remoteEndPoint);
                });
            }
            else
            {
                httpListenerContext.Response.StatusCode = 400;
                httpListenerContext.Response.Close();
            }
        }
    }
}