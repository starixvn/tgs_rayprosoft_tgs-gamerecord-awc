namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    public record AwcLudoCommand(Uri GameRecordApiUrl, Guid GameProviderId, string GameProviderCode, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime, DateTimeOffset EndTime)
        : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderId, GameProviderCode, GameProviderAgentId, GameProviderCert, StartTime, EndTime);
}
