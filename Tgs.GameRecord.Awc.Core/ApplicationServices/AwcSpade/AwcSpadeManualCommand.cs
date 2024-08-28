namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSpade;

public record AwcSpadeManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
