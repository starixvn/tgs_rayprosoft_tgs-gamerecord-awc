using System.Reflection;
using StackExchange.Redis;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordCore(
		this IServiceCollection services,
		Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
		Action<GameInterfaceAwcClientOptions, IServiceProvider> gameInterfaceAwcClientOptions,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddGameCommandClient(gameCommandClientOptions)
		.AddGameInterfaceAwcClient(gameInterfaceAwcClientOptions)
		.AddTransient<IHangfireUtensil, HangfireUtensil>()
		.AddTransient<IHangfireLock>(sp => ActivatorUtilities.CreateInstance<HangfireLock>(sp, redisDatabaseResolver(sp)))
		.AddSingleton<IGameRecordManualDispatcher, GameRecordManualDispatcher>()
		.AddSingleton<IGameRecordScheduleDispatcher, GameRecordScheduleDispatcher>();
}
