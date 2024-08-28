namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAelotto;

public record AwcAelottoScheduleCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime);
