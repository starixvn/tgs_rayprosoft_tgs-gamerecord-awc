namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPgs;

public record AwcPgsScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
