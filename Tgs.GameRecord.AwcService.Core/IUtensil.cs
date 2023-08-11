using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.AwcService.Core;

public interface IUtensil
{
    GameRecordClientStatus GetClientStatus(
        bool isClosed,
        bool isVoid,
        decimal playerWinLose);

    ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default);

    bool IsClosed(int txStatus);

    bool IsVoid(int txStatus, int settleStatus);
}
