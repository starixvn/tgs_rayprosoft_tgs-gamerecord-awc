using Tgs.GameInterface.Awc.Client;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcCockfightWorkerService(
    IGameInterfaceAwcRecordClient client,
    ICommandService<PullRecordCommand> service) : ICommandService<AwcCockfightCommand>
{
    public async ValueTask ExecuteAsync(AwcCockfightCommand command, CancellationToken cancellationToken = default)
    {
        var infos = await client.GameUnsettleByAwcCockfightAsync(
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

        infos = await client.GameSettleByAwcCockfightAsync(
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
