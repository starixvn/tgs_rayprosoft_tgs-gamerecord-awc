using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Awc.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcLudo;

internal class AwcLudoManualCommandService(
	ILogger<AwcLudoManualCommandService> logger,
	IGameInterfaceAwcRecordClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<AwcLudoManualCommand>
{
	public async ValueTask ExecuteAsync(AwcLudoManualCommand command, CancellationToken cancellationToken = default)
	{
		var infos = client.GameSettleByAwcLudoAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			cert: command.Cert,
			startTime: command.StartTime,
			endTime: command.EndTime,
			cancellationToken: cancellationToken);

		await foreach (var info in infos)
		{
			try
			{
				await service.ExecuteAsync(
					command: new HandleRecordCommand(
						GameProviderCode: command.GameProviderCode,
						Info: info),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{logError}", new
				{
					LogOn = DateTime.Now.ToString("u"),
					command.GameProviderCode,
					Data = new
					{
						info.PlayerCode,
						info.ExternalRoundId,
						ExternalBetId = info.ExternalWagerId,
						info.ExternalGameId,
						ExternalCreatedOn = info.ExternalCreatedOn.ToDateTimeOffset().ToString("u"),
						ExternalClosedOn = info.ExternalClosedOn.ToDateTimeOffset().ToString("u")
					}
				});
			}
		}
	}
}
