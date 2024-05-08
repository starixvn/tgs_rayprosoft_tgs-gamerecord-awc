namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcFsCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
