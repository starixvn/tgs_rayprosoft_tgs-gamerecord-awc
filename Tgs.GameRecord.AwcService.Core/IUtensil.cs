using Tgs.GameCommand.Common;
using Tgs.GameRounds.WorkService.Common.Protos;

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

    RoundBiddingClientStatus GetGameRoundClientStatus(RoundBiddingStatus gameRoundStatus, decimal playerWinLose);

    RoundBiddingStatus GetGameRoundStatus(int txStatus, int settleStatus);

    ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
