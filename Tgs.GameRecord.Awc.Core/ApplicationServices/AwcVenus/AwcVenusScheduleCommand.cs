namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcVenus;

public record AwcVenusScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
