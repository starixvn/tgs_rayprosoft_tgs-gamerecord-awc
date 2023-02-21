using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

internal class AwcCockfightWorkerService : ICommandService<AwcCockfightCommand>
{
    private readonly IGameInterfaceAwcRecordServiceClient _client;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;
    private readonly ICommandService<GameUnsettleCommand> _unsettleCommandService;
    private readonly IUtensil _utensil;

    public AwcCockfightWorkerService(IGameInterfaceAwcRecordServiceClient client,
        ICommandService<GameSettleCommand> settleCommandService,
        ICommandService<GameUnsettleCommand> unsettleCommandService,
        IUtensil utensil)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
        _unsettleCommandService = unsettleCommandService ?? throw new ArgumentNullException(nameof(unsettleCommandService));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(AwcCockfightCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = "AWC_COCKFIGHT";
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameUnsettle = await _client
            .GameUnsettleByAwcCockfightAsync(
                apiUrl: command.GameRecordApiUrl,
                agentId: command.GameProviderAgentId,
                cert: command.GameProviderCert,
                timeFrom: command.StartTime,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameUnsettle.Items.Any())
        {
            await _unsettleCommandService.ExecuteAsync(new GameUnsettleCommand(
                  GameProviderId: gameProviderId,
                  GameProviderCode: gameProviderCode,
                  Records: gameUnsettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
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
            await _settleCommandService.ExecuteAsync(new GameSettleCommand(
                  GameProviderId: gameProviderId,
                  GameProviderCode: gameProviderCode,
                  Records: gameSettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
