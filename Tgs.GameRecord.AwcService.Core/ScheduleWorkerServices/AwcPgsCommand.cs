namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    public record AwcPgsCommand(Uri GameRecordApiUrl, string GameProviderAgentId, string GameProviderCert, DateTimeOffset TimeFrom)
        : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderAgentId, GameProviderCert, TimeFrom);
}
