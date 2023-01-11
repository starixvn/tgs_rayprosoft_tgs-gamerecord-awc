namespace Tgs.GameRecord.AwcManualService.ViewModels;

public record GameRecordQueryViewModel(
    Uri GameRecordApiUrl,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
