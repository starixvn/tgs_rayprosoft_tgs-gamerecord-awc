namespace Tgs.GameRecord.AwcService.Core
{
    public record GameRecordManualCommand(
        Uri GameRecordApiUrl,
        string GameProviderAgentId,
        string GameProviderCert,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
