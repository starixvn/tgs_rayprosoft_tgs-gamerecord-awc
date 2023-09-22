namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcPgsCommand(Uri GameRecordApiUrl, string GameProviderCode, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime)
    : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderCode, GameProviderAgentId, GameProviderCert, StartTime);
