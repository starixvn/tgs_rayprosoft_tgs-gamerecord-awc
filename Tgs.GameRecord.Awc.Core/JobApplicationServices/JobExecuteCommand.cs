namespace Tgs.GameRecord.Awc.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string AgentId,
	string Cert);
