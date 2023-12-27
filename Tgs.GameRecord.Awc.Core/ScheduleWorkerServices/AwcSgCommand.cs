namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcSgCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
