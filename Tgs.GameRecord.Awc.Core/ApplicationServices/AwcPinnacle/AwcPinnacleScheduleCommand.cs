namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPinnacle;

public record AwcPinnacleScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
