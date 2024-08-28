namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFs;

public record AwcFsScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
