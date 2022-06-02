namespace Tgs.GameRecord.AwcService.Core.DispatchServices
{
    public record ScheduleDispatchCommand(
        Uri GameRecordApiUrl,
        string GameProviderCode,
        string BrandCode,
        string GameProviderAgentId,
        string GameProviderCert);
}
