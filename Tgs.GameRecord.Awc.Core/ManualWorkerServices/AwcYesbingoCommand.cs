namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

public record AwcYesbingoCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
