namespace Tgs.GameRecord.AwcService.Core
{
    public record GameRecordMuaualCommand(
        Uri GameRecordApiUrl,
        string GameProviderAgentId,
        string GameProviderCert,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
