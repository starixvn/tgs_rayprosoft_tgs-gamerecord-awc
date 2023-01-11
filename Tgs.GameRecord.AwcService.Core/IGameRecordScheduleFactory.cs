namespace Tgs.GameRecord.AwcService.Core;

public interface IGameRecordScheduleFactory
{
    ValueTask ExecuteAsync(GameRecordScheduleCommand command, CancellationToken cancellationToken = default);
}
