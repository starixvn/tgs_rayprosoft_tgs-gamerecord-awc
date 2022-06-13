namespace Tgs.GameRecord.AwcService.Core
{
    public interface IGameRecordScheduleFactory
    {
        ValueTask ExecuteAsync(string gameProviderCode, GameRecordScheduleCommand command, CancellationToken cancellationToken = default);
    }
}
