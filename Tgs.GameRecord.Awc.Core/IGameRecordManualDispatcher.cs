namespace Tgs.GameRecord.Awc.Core;

public interface IGameRecordManualDispatcher
{
	ValueTask ExecuteAsync(GameRecordManualCommand command, CancellationToken cancellationToken = default);
}
