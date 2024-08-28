namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcLudo;

public record AwcLudoManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
