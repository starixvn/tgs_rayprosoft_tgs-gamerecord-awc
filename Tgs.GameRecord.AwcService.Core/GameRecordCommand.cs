namespace Tgs.GameRecord.AwcService.Core
{
    public record GameRecordCommand(
        Uri GameRecordApiUrl,
        string GameProviderAgentId,
        string GameProviderCert,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
