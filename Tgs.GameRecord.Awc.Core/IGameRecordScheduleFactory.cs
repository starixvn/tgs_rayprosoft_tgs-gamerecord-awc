namespace Tgs.GameRecord.Awc.Core;

public interface IGameRecordScheduleFactory
{
    ValueTask ExecuteAsync(GameRecordScheduleCommand command, CancellationToken cancellationToken = default);
}
