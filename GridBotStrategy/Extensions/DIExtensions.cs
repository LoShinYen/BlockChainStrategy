using GridBotStrategy.Services;
using GridBotStrategy.Services.Handlers;

namespace GridBotStrategy.Extensions
{
    internal static class DIExtensions
    {
        internal static IServiceCollection AddServicesAndDependencies(this IServiceCollection services, string? connectionString)
        {
            services.AddMappings();
            services.AddRepositories();
            services.AddServices();
            services.AddHelpers();
            services.AddDatabase(connectionString);
            return services;
        }

        private static IServiceCollection AddMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(GridTradeMappingProfile));
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IGridTradeRobotRepository, GridTradeRobotRepository>();
            services.AddScoped<IGridTradeRobotDetailRepository, GridTradeRobotDetailRepository>();
            services.AddScoped<IGridTradeRobotOrderRepository, GridTradeRobotOrderRepository>();
            services.AddScoped<IGridTradeRobotOrderHostoryRepository, GridTradeRobotOrderHostoryRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRobotManagerService, RobotManagerService>();
            services.AddScoped<TradeExecutionService>();
            services.AddScoped<ITradeHandler, TradeHandler>();
            return services;
        }

        private static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddScoped<MarketDataSubscriptionHandler>();
            services.AddScoped<HeartbeatHandler>();
            services.AddScoped<IMarketDataService, MarketDataService>();
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<CryptoPlatformDbContext>(options =>
            {
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)));
            });
            return services;
        }

    }
}
