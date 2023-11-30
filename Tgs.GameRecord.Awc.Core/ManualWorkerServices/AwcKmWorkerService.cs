using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcKmWorkerService(
    IGameInterfaceAwcRecordServiceClient client,
    ICommandService<GameSettleCommand> settleCommandService) : ICommandService<AwcKmCommand>
{
    public async ValueTask ExecuteAsync(AwcKmCommand command, CancellationToken cancellationToken = default)
    {
        var gameSettle = await client.GameSettleByAwcKmAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameSettle.Items.Count != 0)
        {
            await settleCommandService.ExecuteAsync(
                command: new GameSettleCommand(
                    GameProviderCode: command.GameProviderCode,
                    Records: gameSettle.Items),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
