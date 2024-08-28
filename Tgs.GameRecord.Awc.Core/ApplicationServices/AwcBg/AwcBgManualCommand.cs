namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcBg;

public record AwcBgManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
