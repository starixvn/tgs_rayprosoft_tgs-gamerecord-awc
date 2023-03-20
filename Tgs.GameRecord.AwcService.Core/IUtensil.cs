using Tgs.GameCommand.Common;
using Tgs.GameRounds.Work.Common.Protos;

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

    RoundClientStatus GetGameRoundClientStatus(RoundStatus gameRoundStatus, decimal playerWinLose);

    RoundStatus GetGameRoundStatus(int txStatus, int settleStatus);

    ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
