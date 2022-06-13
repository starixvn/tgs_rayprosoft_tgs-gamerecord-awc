namespace Tgs.GameRecord.AwcService.Core
{
    public interface IGameRecordMuaualFactory
    {
        ValueTask ExecuteAsync(string gameProviderCode, GameRecordMuaualCommand command, CancellationToken cancellationToken = default);
    }
}
