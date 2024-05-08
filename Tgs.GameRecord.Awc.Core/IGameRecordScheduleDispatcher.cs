namespace Tgs.GameRecord.Awc.Core;

public interface IGameRecordScheduleDispatcher
{
	ValueTask ExecuteAsync(GameRecordScheduleCommand command, CancellationToken cancellationToken = default);
}
