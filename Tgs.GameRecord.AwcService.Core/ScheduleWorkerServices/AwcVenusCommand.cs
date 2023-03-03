namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices;

public record AwcVenusCommand(Uri GameRecordApiUrl, Guid GameProviderId, string GameProviderCode, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime)
    : GameRecordScheduleCommand(GameRecordApiUrl, GameProviderId, GameProviderCode, GameProviderAgentId, GameProviderCert, StartTime);
