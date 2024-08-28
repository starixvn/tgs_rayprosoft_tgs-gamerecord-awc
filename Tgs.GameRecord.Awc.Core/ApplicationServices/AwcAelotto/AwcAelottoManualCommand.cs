namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAelotto;

public record AwcAelottoManualCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
