using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwcServiceCore(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameInterfaceAwcClientOptions, IServiceProvider> gameInterfaceAwcClientOptions)
        => services
        .AddOptions<RedisOptions>()
        .Configure(redisDBconfigure)
        .Services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(gameRecordInfoOptions)
        .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddSingleton<RedisConnectionManager>()
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameInterfaceAwcClient(gameInterfaceAwcClientOptions)
        .AddGameRecordHandleClient()
        .AddTransient<IHangfireUtensil, HangfireUtensil>()
        .AddSingleton<IGameRecordManualDispatcher, GameRecordManualDispatcher>()
        .AddSingleton<IGameRecordScheduleDispatcher, GameRecordScheduleDispatcher>();
}
