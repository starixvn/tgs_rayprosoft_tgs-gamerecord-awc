using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.MuaualWorkerServices
{
    internal class AwcYlWorkerService : ICommandService<AwcYlCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ICommandService<GaemSettleCommand> _settleCommandService;
        private readonly IUtensil _utensil;

        public AwcYlWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ICommandService<GaemSettleCommand> settleCommandService,
            IUtensil utensil)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _settleCommandService = settleCommandService ?? throw new ArgumentNullException(nameof(settleCommandService));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(AwcYlCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = GameProviderType.AWC_YL;
            var gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

            var gameSettle = await _gameInterfaceAwcRecordServiceGrpc.GameSettleByAwcYlAsync(
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
