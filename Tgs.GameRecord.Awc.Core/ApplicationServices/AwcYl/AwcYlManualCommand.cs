namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYl;

public record AwcYlManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
