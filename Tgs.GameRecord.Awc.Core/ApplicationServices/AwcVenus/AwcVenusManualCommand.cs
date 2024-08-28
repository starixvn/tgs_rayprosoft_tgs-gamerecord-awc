namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcVenus;

public record AwcVenusManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
