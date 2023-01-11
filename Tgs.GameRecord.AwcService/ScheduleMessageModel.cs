namespace Tgs.GameRecord.AwcService;

public record ScheduleMessageModel(
    Uri GameRecordApiUrl,
    Guid GameProviderId,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert);
