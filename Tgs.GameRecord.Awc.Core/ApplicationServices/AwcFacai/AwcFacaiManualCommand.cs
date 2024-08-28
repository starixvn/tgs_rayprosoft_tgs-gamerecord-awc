namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFacai;

public record AwcFacaiManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
