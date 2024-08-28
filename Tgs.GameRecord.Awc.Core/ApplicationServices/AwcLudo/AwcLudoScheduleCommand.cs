namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcLudo;

public record AwcLudoScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
