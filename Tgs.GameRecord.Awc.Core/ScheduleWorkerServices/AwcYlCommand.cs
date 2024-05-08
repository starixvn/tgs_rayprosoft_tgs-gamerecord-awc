namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcYlCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
