using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcAwsWorkerService(
	IGameInterfaceAwcRecordClient client,
	ICommandService<PullRecordCommand> service) : ICommandService<AwcAwsCommand>
{
	public async ValueTask ExecuteAsync(AwcAwsCommand command, CancellationToken cancellationToken = default)
	{
		var infos = await client.GameSettleByAwcAwsAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			cert: command.Cert,
			startTime: command.StartTime,
			endTime: command.EndTime,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameWagerInfo>();

		if (infos.Any())
		{
			await service.ExecuteAsync(
				command: new PullRecordCommand(
					GameProviderCode: command.GameProviderCode,
					Infos: infos),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
