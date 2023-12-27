namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcAelottoCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime);
