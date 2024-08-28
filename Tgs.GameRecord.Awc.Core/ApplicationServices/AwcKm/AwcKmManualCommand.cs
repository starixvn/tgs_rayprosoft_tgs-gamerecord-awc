namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcKm;

public record AwcKmManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
