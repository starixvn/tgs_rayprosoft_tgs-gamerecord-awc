using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Awc.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPgs;

internal class AwcPgsManualCommandService(
	ILogger<AwcPgsManualCommandService> logger,
	IGameInterfaceAwcRecordClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<AwcPgsManualCommand>
{
	public async ValueTask ExecuteAsync(AwcPgsManualCommand command, CancellationToken cancellationToken = default)
	{
		var infos = client.GameSettleByAwcPgsAsync(
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
