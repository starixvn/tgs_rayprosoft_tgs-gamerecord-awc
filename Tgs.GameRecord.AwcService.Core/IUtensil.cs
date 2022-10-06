using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameRecord.AwcService.Core
{
    public interface IUtensil
    {
        ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

        ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId);

        ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

        GameRoundClientStatus GetGameRoundClientStatus(GameRoundStatus gameRoundStatus, decimal playerWinLose);

        GameRoundStatus GetGameRoundStatus(int txStatus, int settleStatus);

        ValueTask<PlayerModel?> GetPlayerAsync(string playerCode);
    }
}
