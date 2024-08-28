using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Awc.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices.AwcBg;

internal class AwcBgScheduleCommandService(
	ILogger<AwcBgScheduleCommandService> logger,
	IGameInterfaceAwcRecordClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<AwcBgScheduleCommand>
{
	public async ValueTask ExecuteAsync(AwcBgScheduleCommand command, CancellationToken cancellationToken = default)
	{
		var infos = client.GamePullRecordByAwcBgAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			cert: command.Cert,
			time_from: command.StartTime,
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
