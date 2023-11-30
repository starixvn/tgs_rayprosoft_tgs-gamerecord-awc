using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcRcbWorkerService(IGameInterfaceAwcRecordServiceClient client,
    ICommandService<GameSettleCommand> settleCommandService,
    ICommandService<GameUnsettleCommand> unsettleCommandService) : ICommandService<AwcRcbCommand>
{
    public async ValueTask ExecuteAsync(AwcRcbCommand command, CancellationToken cancellationToken = default)
    {
        var gameUnsettle = await client
            .GameUnsettleByAwcRcbAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameUnsettle.Items.Count != 0)
        {
            await unsettleCommandService.ExecuteAsync(
                command: new GameUnsettleCommand(
                    GameProviderCode: command.GameProviderCode,
                    Records: gameUnsettle.Items),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        var gameSettle = await client.GameSettleByAwcRcbAsync(
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
