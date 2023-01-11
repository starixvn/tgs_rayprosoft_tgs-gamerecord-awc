using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

internal class AwcAwsWorkerservice : ICommandService<AwcAwsCommand>
{
    private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
    private readonly ICommandService<GameSettleCommand> _settleCommandService;
    private readonly IUtensil _utensil;

    public AwcAwsWorkerservice(
        IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
        ICommandService<GameSettleCommand> settleCommandService,
        IUtensil utensil)
    {
        _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
        _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(AwcAwsCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = GameProviderType.AWC_AWS;
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameSettle = await _gameInterfaceAwcRecordServiceGrpc
            .GameSettleByAwcAwsAsync(
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
