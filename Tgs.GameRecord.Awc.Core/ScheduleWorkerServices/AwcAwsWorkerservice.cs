using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

internal class AwcAwsWorkerService(
	ILogger<AwcAwsWorkerService> logger,
	IGameInterfaceAwcRecordClient client,
	ICommandService<PullRecordCommand> service) : ICommandService<AwcAwsCommand>
{
	public async ValueTask ExecuteAsync(AwcAwsCommand command, CancellationToken cancellationToken = default)
	{
		var infos = await client.GamePullRecordByAwcAwsAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			cert: command.Cert,
			time_from: command.StartTime,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerInfo>();

		if (!infos.Any())
		{
			logger.LogInformation("LogOn:{logOn} Message: The records are empty.({command})", DateTime.Now.ToString("o"), command);
			return;
		}

		await service.ExecuteAsync(
			command: new PullRecordCommand(
				GameProviderCode: command.GameProviderCode,
				Infos: infos),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
