namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcBg;

public record AwcBgScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
