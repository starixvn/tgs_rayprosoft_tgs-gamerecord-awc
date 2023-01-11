using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

internal class AwcSgWorkerService : ICommandService<AwcSgCommand>
{
    private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;
    private readonly IUtensil _utensil;

    public AwcSgWorkerService(
        IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
        ICommandService<GameSettleCommand> settleCommandService,
        IUtensil utensil)
    {
        _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(AwcSgCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = GameProviderType.AWC_SG;
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameSettle = await _gameInterfaceAwcRecordServiceGrpc
            .GameSettleByAwcSgAsync(
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
