namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYl;

public record AwcYlScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
