using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.WorkerServices
{
    internal class AwcFsWorkerService : ICommandService<AwcFsCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ICommandService<GaemSettleCommand> _settleCommandService;
        private readonly IUtensil _utensil;

        public AwcFsWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ICommandService<GaemSettleCommand> settleCommandService,
            IUtensil utensil)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(AwcFsCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = GameProviderType.AWC_FS;
            var gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            var gameSettle = await _gameInterfaceAwcRecordServiceGrpc.GameSettleByAwcFsAsync(
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
