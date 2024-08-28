namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record SchedulePullRecordCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
