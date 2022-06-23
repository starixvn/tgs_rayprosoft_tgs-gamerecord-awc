namespace Tgs.GameRecord.AwcService
{
    public record ManualMessageRequest(
        Uri GameRecordApiUrl,
        string GameProviderCode,
        string GameProviderAgentId,
        string GameProviderCert,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
