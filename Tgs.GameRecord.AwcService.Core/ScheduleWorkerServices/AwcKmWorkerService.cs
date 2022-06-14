using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    internal class AwcKmWorkerService : ICommandService<AwcKmCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;
        private readonly IUtensil _utensil;

        public AwcKmWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ICommandService<GamePullRecordCommand> pullRecordCommandService,
            IUtensil utensil)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(AwcKmCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderCode = GameProviderType.AWC_KM;
            var timeFromDateTime = (DateTimeOffset?)null;
            var gameProviderId = await _utensil.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
            timeFromDateTime = command.TimeFrom;

            while (timeFromDateTime is not null)
            {
                var gamePullRecordReply = await _gameInterfaceAwcRecordServiceGrpc.GamePullRecordByAwcKmAsync(
                    command.GameRecordApiUrl,
                    command.GameProviderAgentId,
                    command.GameProviderCert,
                    command.TimeFrom,
                    cancellationToken).ConfigureAwait(false);

                if (gamePullRecordReply.Items is not null && gamePullRecordReply.Items.Any())
                {
                    await _pullRecordCommandService.ExecuteAsync(new GamePullRecordCommand(
                      GameProviderId: gameProviderId,
                      GameProviderCode: gameProviderCode,
                      Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);

                    timeFromDateTime = DateTimeOffset.ParseExact(
                        gamePullRecordReply.Items.OrderByDescending(x => x.UpdateTime).Single().UpdateTime,
                        @"yyyy-MM-dd\THH:mm:ssK",
                        null,
                        styles: System.Globalization.DateTimeStyles.RoundtripKind);
                }
                else
                {
                    timeFromDateTime = null;
                }
            }
        }
    }
}
