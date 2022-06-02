namespace Tgs.GameRecord.AwcService.Core
{
    public interface IGameRecordFactory
    {
        ValueTask ExecuteAsync(string gameProviderCode, GameRecordCommand command, CancellationToken cancellationToken = default);
    }
}
