using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcFacaiWorkerService : ICommandService<AwcFacaiCommand>
{
    private readonly IGameInterfaceAwcRecordServiceClient _client;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;

    public AwcFacaiWorkerService(
        IGameInterfaceAwcRecordServiceClient client,
        ICommandService<GameSettleCommand> settleCommandService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
    }

    public async ValueTask ExecuteAsync(AwcFacaiCommand command, CancellationToken cancellationToken = default)
    {
        var gameSettle = await _client
            .GameSettleByAwcFacaiAsync(
                apiUrl: command.GameRecordApiUrl,
                agentId: command.GameProviderAgentId,
                cert: command.GameProviderCert,
                startTime: command.StartTime,
                endTime: command.EndTime,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameSettle.Items.Any())
        {
            var recordCommand = new GameSettleCommand(
                GameProviderCode: command.GameProviderCode,
                Records: gameSettle.Items);

            await _settleCommandService
                .ExecuteAsync(
                command: recordCommand,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
