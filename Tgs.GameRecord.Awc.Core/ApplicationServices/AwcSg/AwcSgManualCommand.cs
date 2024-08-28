namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSg;

public record AwcSgManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
