namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAws;

public record AwcAwsScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
