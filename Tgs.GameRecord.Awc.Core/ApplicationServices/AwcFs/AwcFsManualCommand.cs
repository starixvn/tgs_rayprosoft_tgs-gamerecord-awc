namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFs;

public record AwcFsManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
