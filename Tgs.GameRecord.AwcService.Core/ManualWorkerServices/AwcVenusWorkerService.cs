using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

internal class AwcVenusWorkerService : ICommandService<AwcVenusCommand>
{
    private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;
    private readonly IUtensil _utensil;

    public AwcVenusWorkerService(
        IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
        ICommandService<GameSettleCommand> settleCommandService,
        IUtensil utensil)
    {
        _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(AwcVenusCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = GameProviderType.AWC_VENUS;
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameSettle = await _gameInterfaceAwcRecordServiceGrpc
            .GameSettleByAwcVenusAsync(
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
                  Records: gameSettle.Items), cancellationToken).ConfigureAwait(false);
        }
    }
}
