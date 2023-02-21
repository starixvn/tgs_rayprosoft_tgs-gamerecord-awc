using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.AwcService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseRegister(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var gameRecordInfo = app.Configuration.GetSection("GameRecordInfo").Get<GameRecordInfoOptions>();
            var utensil = scope.ServiceProvider.GetRequiredService<IUtensil>();

            var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();

            var gameProviderIdInfos = await client
                .GameProviderGetGameProviderIdsAsync()
                .ToArrayAsync()
                .ConfigureAwait(false)
                ?? Enumerable.Empty<GameProviderIdInfo>();

            foreach (var gameProviderIdInfo in gameProviderIdInfos)
            {
                if (gameProviderIdInfo.GameProviderCode.StartsWith(gameRecordInfo.GameProviderCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = await utensil
                        .GetGameProviderIdAsync(
                            gameProviderIdInfo.GameProviderCode)
                        .ConfigureAwait(false);
                }
            }

            foreach (var gameProviderIdInfo in gameProviderIdInfos)
            {
                if (gameProviderIdInfo.GameProviderCode.StartsWith(gameRecordInfo.GameProviderCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = await utensil
                        .GetGameProviderIdAsync(
                            gameProviderIdInfo.GameProviderCode)
                        .ConfigureAwait(false);

                    var games = client
                        .GameQueryAsync(
                        gameProviderCode: gameRecordInfo.GameProviderCode)
                        .ToEnumerable()
                        ?? Enumerable.Empty<GameInfo>();

                    foreach (var game in games)
                    {
                        _ = await utensil
                            .GetGameIdAsync(
                                gameProviderId: gameProviderIdInfo.GameProviderId,
                                gameExternalId: game.GameExternalId)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        return app;
    }
}
