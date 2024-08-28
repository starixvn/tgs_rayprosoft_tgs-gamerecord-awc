namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcCockfight;

public record AwcCockfightManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
