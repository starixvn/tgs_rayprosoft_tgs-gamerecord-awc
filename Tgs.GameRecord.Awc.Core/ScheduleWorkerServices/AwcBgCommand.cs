namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcBgCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
