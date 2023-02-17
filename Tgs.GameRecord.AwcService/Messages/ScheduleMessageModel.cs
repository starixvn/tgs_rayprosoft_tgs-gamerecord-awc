namespace Tgs.GameRecord.AwcService.Messages;

public record ScheduleMessageModel(
    Uri GameRecordApiUrl,
    Guid GameProviderId,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert);
