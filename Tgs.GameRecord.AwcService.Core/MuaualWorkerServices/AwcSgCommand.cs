namespace Tgs.GameRecord.AwcService.Core.MuaualWorkerServices
{
    public record AwcSgCommand(Uri GameRecordApiUrl, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime, DateTimeOffset EndTime)
        : GameRecordMuaualCommand(GameRecordApiUrl, GameProviderAgentId, GameProviderCert, StartTime, EndTime);
}
