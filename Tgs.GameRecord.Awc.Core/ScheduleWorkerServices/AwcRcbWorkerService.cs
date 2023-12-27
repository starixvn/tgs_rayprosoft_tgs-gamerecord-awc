using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

internal class AwcRcbWorkerService(
    ILogger<AwcRcbWorkerService> logger,
    IGameInterfaceAwcRecordClient client,
    ICommandService<PullRecordCommand> service) : ICommandService<AwcRcbCommand>
{
    public async ValueTask ExecuteAsync(AwcRcbCommand command, CancellationToken cancellationToken = default)
    {
        var infos = await client.GamePullRecordByAwcRcbAsync(
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
