using Tgs.GameCommand.Common;
using Tgs.GameRounds.Common;

namespace Tgs.GameRecord.AwcService.Core;

public interface IUtensil
{
    ValueTask<Guid?> GetBrandGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default);

    ValueTask<int?> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default);

    ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default);

    GameRoundClientStatus GetGameRoundClientStatus(GameRoundStatus gameRoundStatus, decimal playerWinLose);

    GameRoundStatus GetGameRoundStatus(int txStatus, int settleStatus);

    ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
