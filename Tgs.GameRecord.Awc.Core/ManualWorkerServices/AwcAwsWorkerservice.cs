using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcAwsWorkerservice(
    IGameInterfaceAwcRecordServiceClient client,
    ICommandService<GameSettleCommand> settleCommandService) : ICommandService<AwcAwsCommand>
{
    public async ValueTask ExecuteAsync(AwcAwsCommand command, CancellationToken cancellationToken = default)
    {
        var gameSettle = await client.GameSettleByAwcAwsAsync(
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
