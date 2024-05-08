using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcBgWorkerService(
	IGameInterfaceAwcRecordClient client,
	ICommandService<PullRecordCommand> service) : ICommandService<AwcBgCommand>
{
	public async ValueTask ExecuteAsync(AwcBgCommand command, CancellationToken cancellationToken = default)
	{
		var infos = await client.GameSettleByAwcBgAsync(
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
