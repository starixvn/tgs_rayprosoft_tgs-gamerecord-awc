using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.MuaualWorkerServices
{
    internal class AwcRcbWorkerService : ICommandService<AwcRcbCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ICommandService<GaemSettleCommand> _settleCommandService;
        private readonly ICommandService<GaemUnsettleCommand> _unsettleCommandService;
        private readonly IUtensil _utensil;

        public AwcRcbWorkerService(IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ICommandService<GaemSettleCommand> settleCommandService,
            ICommandService<GaemUnsettleCommand> unsettleCommandService,
            IUtensil utensil)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
            _unsettleCommandService = unsettleCommandService ?? throw new ArgumentNullException(nameof(unsettleCommandService));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(AwcRcbCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = GameProviderType.AWC_RCB;
            var gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            var gameUnsettle = await _gameInterfaceAwcRecordServiceGrpc.GameUnsettleByAwcRcbAsync(
                command.GameRecordApiUrl,
                command.GameProviderAgentId,
                command.GameProviderCert,
                command.StartTime,
                command.EndTime,
                cancellationToken).ConfigureAwait(false);

            if (gameUnsettle.Items.Any())
            {
                await _unsettleCommandService.ExecuteAsync(new GaemUnsettleCommand(
                      GameProviderId: gameProviderId,
                      GameProviderCode: gameProviderCode,
                      Records: gameUnsettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var gameSettle = await _gameInterfaceAwcRecordServiceGrpc.GameSettleByAwcRcbAsync(
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
                      Records: gameSettle.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
