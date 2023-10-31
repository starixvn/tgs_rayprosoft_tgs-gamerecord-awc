using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

internal class AwcCockfightWorkerService : ICommandService<AwcCockfightCommand>
{
    private readonly IGameInterfaceAwcRecordServiceClient _client;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;
    private readonly ICommandService<GameUnsettleCommand> _unsettleCommandService;

    public AwcCockfightWorkerService(IGameInterfaceAwcRecordServiceClient client,
        ICommandService<GameSettleCommand> settleCommandService,
        ICommandService<GameUnsettleCommand> unsettleCommandService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
        _unsettleCommandService = unsettleCommandService ?? throw new ArgumentNullException(nameof(unsettleCommandService));
    }

    public async ValueTask ExecuteAsync(AwcCockfightCommand command, CancellationToken cancellationToken = default)
    {
        var gameUnsettle = await _client
            .GameUnsettleByAwcCockfightAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameUnsettle.Items.Any())
        {
            await _unsettleCommandService
                .ExecuteAsync(
                command: new GameUnsettleCommand(
                    GameProviderCode: command.GameProviderCode,
                    Records: gameUnsettle.Items),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        var gameSettle = await _client
            .GameSettleByAwcCockfightAsync(
                apiUrl: command.GameRecordApiUrl,
                agentId: command.GameProviderAgentId,
                cert: command.GameProviderCert,
                startTime: command.StartTime,
                endTime: command.EndTime,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameSettle.Items.Any())
        {
            await _settleCommandService
                .ExecuteAsync(
                command: new GameSettleCommand(
                    GameProviderCode: command.GameProviderCode,
                    Records: gameSettle.Items),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
