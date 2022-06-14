using System.Reflection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.AwcService.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwcServiceCore(
            this IServiceCollection services,
             Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
             Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
             Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
             Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions,
             Func<IServiceProvider, IDatabase> redisDatabaseResolver
            )
            => services
                .AddOptions<GameRecordInfoOptions>()
                .Configure(gameRecordInfoOptions)
                .Services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddGameProvidersClient(gameProvidersClientOptions)
                .AddSingleton<IDistributedLockFactory, RedLockFactory>(sp =>
                {
                    var database = redisDatabaseResolver(sp);
                    var multiplexers = new List<RedLockMultiplexer>
                    {
                        (ConnectionMultiplexer)database.Multiplexer
                    };
                    return RedLockFactory.Create(multiplexers);
                })
                .AddScoped<IGameRecordManualFactory, GameRecordManualFactory>()
                .AddScoped<IGameRecordScheduleFactory, GameRecordScheduleFactory>()
                .AddTransient<IUtensil, UtensilRepository>()
                .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
    }
}
