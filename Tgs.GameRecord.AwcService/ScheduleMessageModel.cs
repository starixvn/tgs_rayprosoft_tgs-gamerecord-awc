namespace Tgs.GameRecord.KaiyuanService
{
    public record ScheduleMessageModel(
        Uri GameRecordApiUrl,
        string GameProviderCode,
        string BrandCode,
        string GameProviderAgentId,
        string GameProviderCert);
}
