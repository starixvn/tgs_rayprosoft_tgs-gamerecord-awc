namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcPgsCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
