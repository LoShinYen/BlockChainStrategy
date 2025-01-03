using GridBotStrategy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        // 建立主機（Host）來管理應用程式生命週期與依賴注入
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<CryptoPlatformDbContext>(options =>
                {
                    options.UseMySql("server=127.0.0.1;port=3306;database=crypto_platform;user=root;password=",
                        new MySqlServerVersion(new Version(8, 0, 32)));
                });


                services.AddScoped<IGridTradeRobotRepository, GridTradeRobotRepository>();

                // 註冊 Service
                services.AddScoped<IRobotManagerService, RobotManagerService>();
            })
            .Build();

        // 使用 DI 容器解析服務並執行
        var service = host.Services.GetRequiredService<IRobotManagerService>();
        await service.ExcuteAsync();
    }
}