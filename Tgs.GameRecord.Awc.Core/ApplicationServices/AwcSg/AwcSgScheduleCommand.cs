namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSg;

public record AwcSgScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
