namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record ManualPullRecordCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
