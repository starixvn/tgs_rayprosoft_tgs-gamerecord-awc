namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcKm;

public record AwcKmScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
