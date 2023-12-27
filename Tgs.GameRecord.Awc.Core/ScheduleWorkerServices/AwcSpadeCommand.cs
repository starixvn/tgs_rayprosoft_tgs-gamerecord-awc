namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcSpadeCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
