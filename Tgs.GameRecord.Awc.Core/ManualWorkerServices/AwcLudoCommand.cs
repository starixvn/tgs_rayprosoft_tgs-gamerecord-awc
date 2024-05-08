namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

public record AwcLudoCommand(
	string GameProviderCode,
	Uri ApiUrl,
	string AgentId,
	string Cert,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
