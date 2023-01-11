namespace Tgs.GameRecord.AwcService.Core;

public interface IGameRecordManualFactory
{
    ValueTask ExecuteAsync(string gameProviderCode, GameRecordManualCommand command, CancellationToken cancellationToken = default);
}
