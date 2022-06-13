namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    public record AwcSpadeCommand(Uri GameRecordApiUrl, string GameProviderAgentId, string GameProviderCert, DateTimeOffset TimeFrom)
        : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderAgentId, GameProviderCert, TimeFrom);
}
