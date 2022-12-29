using System.Reflection;
using StackExchange.Redis;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameRecord.AwcService.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwcServiceCore(
            this IServiceCollection services,
            Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
            Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
            Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
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
                .AddGameCommandClient(gameCommandClientOptions)
                .AddGameProvidersClient(gameProvidersClientOptions)
                .AddScoped<IGameRecordManualFactory, GameRecordManualFactory>()
                .AddScoped<IGameRecordScheduleFactory, GameRecordScheduleFactory>()
                .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, redisDatabaseResolver(sp), TimeSpan.FromMinutes(2)))
                .AddTransient<IUtensil, UtensilRepository>()
                .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
    }
}
