namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYesbingo;

public record AwcYesbingoScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
