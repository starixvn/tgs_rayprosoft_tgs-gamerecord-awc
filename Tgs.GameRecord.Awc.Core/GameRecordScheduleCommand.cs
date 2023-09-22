namespace Tgs.GameRecord.Awc.Core;

public record GameRecordScheduleCommand(
    Uri GameRecordApiUrl,
    string GameProviderCode,
    string GameProviderAgentId,
    string GameProviderCert,
    DateTimeOffset StartTime);
