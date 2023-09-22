namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

public record AwcFsCommand(Uri GameRecordApiUrl, string GameProviderCode, string GameProviderAgentId, string GameProviderCert, DateTimeOffset StartTime, DateTimeOffset EndTime)
    : GameRecordManualCommand(GameRecordApiUrl, GameProviderCode, GameProviderAgentId, GameProviderCert, StartTime, EndTime);
