namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYesbingo;

public record AwcYesbingoManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
