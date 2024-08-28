namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcRcb;

public record AwcRcbScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
