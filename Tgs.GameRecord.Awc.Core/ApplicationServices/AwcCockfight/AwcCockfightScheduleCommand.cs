namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcCockfight;

public record AwcCockfightScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
