namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcVenusCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
