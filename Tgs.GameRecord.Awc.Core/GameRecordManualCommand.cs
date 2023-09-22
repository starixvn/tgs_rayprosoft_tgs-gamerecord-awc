namespace Tgs.GameRecord.Awc.Core;

public record GameRecordManualCommand(
    Uri GameRecordApiUrl,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
