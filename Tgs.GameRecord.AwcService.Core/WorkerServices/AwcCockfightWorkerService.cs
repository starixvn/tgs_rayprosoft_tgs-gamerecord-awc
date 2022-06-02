using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.WorkerServices
{
    internal class AwcCockfightWorkerService : ICommandService<AwcCockfightCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ICommandService<GaemSettleCommand> _settleCommandService;
        private readonly ICommandService<GaemUnsettleCommand> _unsettleCommandService;
        private readonly IUtensil _utensil;

        public AwcCockfightWorkerService(IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ICommandService<GaemSettleCommand> settleCommandService,
            ICommandService<GaemUnsettleCommand> unsettleCommandService,
            IUtensil utensil)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
            _unsettleCommandService = unsettleCommandService ?? throw new ArgumentNullException(nameof(unsettleCommandService));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(AwcCockfightCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = GameProviderType.AWC_COCKFIGHT;
            var gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            var gameUnsettle = await _gameInterfaceAwcRecordServiceGrpc.GameUnsettleByAwcCockfightAsync(
                command.GameRecordApiUrl,
                command.GameProviderAgentId,
                command.GameProviderCert,
                command.StartTime,
                cancellationToken).ConfigureAwait(false);

            if (gameUnsettle.Items.Any())
            {
                await _unsettleCommandService.ExecuteAsync(new GaemUnsettleCommand(
                      GameProviderId: gameProviderId,
                      GameProviderCode: gameProviderCode,
                      Transactions: gameUnsettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var gameSettle = await _gameInterfaceAwcRecordServiceGrpc.GameSettleByAwcCockfightAsync(
                command.GameRecordApiUrl,
                command.GameProviderAgentId,
                command.GameProviderCert,
                command.StartTime,
                command.EndTime,
                cancellationToken).ConfigureAwait(false);

            if (gameSettle.Items.Any())
            {
                await _settleCommandService.ExecuteAsync(new GaemSettleCommand(
                      GameProviderId: gameProviderId,
                      GameProviderCode: gameProviderCode,
                      Transactions: gameSettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
