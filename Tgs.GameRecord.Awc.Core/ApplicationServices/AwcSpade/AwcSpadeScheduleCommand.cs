namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSpade;

public record AwcSpadeScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
