namespace Tgs.GameRecord.AwcService.Core.DispatchServices
{
    public record ScheduleDispatchCommand(
        Uri GameRecordApiUrl,
        Guid GameProviderId,
        string GameProviderCode,
        string GameProviderAgentId,
        string GameProviderCert);
}
