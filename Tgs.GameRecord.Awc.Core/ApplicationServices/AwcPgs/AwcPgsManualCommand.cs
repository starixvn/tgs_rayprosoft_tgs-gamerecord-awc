namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPgs;

public record AwcPgsManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
