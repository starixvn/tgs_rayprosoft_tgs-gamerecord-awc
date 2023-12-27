namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcKmCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
