namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcLudoCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
