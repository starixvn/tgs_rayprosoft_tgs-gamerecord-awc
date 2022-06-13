using System.Reflection;
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
             Action<GameRecordInfoOptions, IServiceProvider> configure,
              Action<GameInterfaceAwcServiceClientOptions, IServiceProvider> gameInterfaceAwcServiceClientOptions,
             Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
             Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions)
            => services
                .AddOptions<GameRecordInfoOptions>()
                .Configure(configure)
                .Services
                .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient)
                .AddGameInterfaceAwcServiceClient(gameInterfaceAwcServiceClientOptions)
                .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
                .AddGameProvidersClient(gameProvidersClientOptions)
                .AddScoped<IGameRecordMuaualFactory, GameRecordMuaualFactory>()
                .AddTransient<IUtensil, UtensilRepository>()
                .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
    }
}
