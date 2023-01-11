namespace Tgs.GameRecord.AwcManualService;

public record ManualMessageRequest(
    Uri GameRecordApiUrl,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
