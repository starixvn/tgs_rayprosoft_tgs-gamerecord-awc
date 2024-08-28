namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAws;

public record AwcAwsManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
