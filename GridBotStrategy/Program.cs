using GridBotStrategy.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

                #region DB
                var connectionString = context.Configuration.GetConnectionString("CryptoPlatformDb");
                services.AddDbContext<CryptoPlatformDbContext>(options =>
                {
                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)));
                });
                #endregion

                #region Repository

                services.AddScoped<IGridTradeRobotRepository, GridTradeRobotRepository>();

                #endregion

                #region Service

                services.AddScoped<IRobotManagerService, RobotManagerService>();

                #endregion
            })
            .Build();

        var robotManagerService = host.Services.GetRequiredService<IRobotManagerService>();
        await robotManagerService.ExcuteAsync();

    }
}