namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcYesbingoCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
