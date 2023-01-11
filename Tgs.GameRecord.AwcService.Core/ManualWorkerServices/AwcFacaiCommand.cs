namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

public record AwcFacaiCommand(Uri GameRecordApiUrl, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime, DateTimeOffset EndTime)
    : GameRecordManualCommand(GameRecordApiUrl, GameProviderAgentId, GameProviderCert, StartTime, EndTime);
