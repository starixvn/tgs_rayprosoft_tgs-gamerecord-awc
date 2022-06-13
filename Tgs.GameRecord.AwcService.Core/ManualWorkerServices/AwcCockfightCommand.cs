namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices
{
    public record AwcCockfightCommand(Uri GameRecordApiUrl, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime, DateTimeOffset EndTime)
        : GameRecordManualCommand(GameRecordApiUrl, GameProviderAgentId, GameProviderCert, StartTime, EndTime);
}
