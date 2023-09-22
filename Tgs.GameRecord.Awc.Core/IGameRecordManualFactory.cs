namespace Tgs.GameRecord.Awc.Core;

public interface IGameRecordManualFactory
{
    ValueTask ExecuteAsync(GameRecordManualCommand command, CancellationToken cancellationToken = default);
}
