namespace Tgs.GameRecord.Awc.Core;

public record GameRecordScheduleCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
