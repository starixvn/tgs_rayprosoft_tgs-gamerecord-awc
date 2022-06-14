namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    public record AwcKmCommand(Uri GameRecordApiUrl, Guid GameProviderId, string GameProviderCode, string GameProviderAgentId, string GameProviderCert, DateTimeOffset TimeFrom)
        : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderId, GameProviderCode, GameProviderAgentId, GameProviderCert, TimeFrom);
}
