namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFacai;

public record AwcFacaiScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
