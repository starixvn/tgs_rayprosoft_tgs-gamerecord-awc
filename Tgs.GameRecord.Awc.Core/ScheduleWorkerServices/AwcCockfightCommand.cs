namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

public record AwcCockfightCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
