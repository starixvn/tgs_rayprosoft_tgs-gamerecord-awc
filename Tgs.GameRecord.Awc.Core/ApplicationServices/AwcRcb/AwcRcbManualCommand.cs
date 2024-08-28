namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcRcb;

public record AwcRcbManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
