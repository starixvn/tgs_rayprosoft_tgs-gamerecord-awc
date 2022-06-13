namespace Tgs.GameRecord.AwcService.Core
{
    public record GameRecordScheduleCommand(
        Uri GameRecordApiUrl,
        string GameProviderAgentId,
        string GameProviderCert,
        DateTimeOffset TimeFrom);
}
