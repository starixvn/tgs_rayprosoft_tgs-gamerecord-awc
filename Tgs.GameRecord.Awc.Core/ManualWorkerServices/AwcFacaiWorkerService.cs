using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcFacaiWorkerService(
    IGameInterfaceAwcRecordServiceClient client,
    ICommandService<GameSettleCommand> settleCommandService) : ICommandService<AwcFacaiCommand>
{
    public async ValueTask ExecuteAsync(AwcFacaiCommand command, CancellationToken cancellationToken = default)
    {
        var gameSettle = await client.GameSettleByAwcFacaiAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameSettle.Items.Count != 0)
        {
            var recordCommand = new GameSettleCommand(
                GameProviderCode: command.GameProviderCode,
                Records: gameSettle.Items);

            await settleCommandService.ExecuteAsync(
                command: recordCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
