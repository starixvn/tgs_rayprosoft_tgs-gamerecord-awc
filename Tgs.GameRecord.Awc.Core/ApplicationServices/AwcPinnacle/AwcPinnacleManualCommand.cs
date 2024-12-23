namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPinnacle;

public record AwcPinnacleManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
