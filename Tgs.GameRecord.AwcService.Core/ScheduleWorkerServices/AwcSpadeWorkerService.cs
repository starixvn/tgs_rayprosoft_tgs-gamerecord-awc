using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    internal class AwcSpadeWorkerService : ICommandService<AwcSpadeCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ILogger<AwcSpadeWorkerService> _logger;
        private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;

        public AwcSpadeWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ILogger<AwcSpadeWorkerService> logger,
            ICommandService<GamePullRecordCommand> pullRecordCommandService)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
        }

        public async ValueTask ExecuteAsync(AwcSpadeCommand command, CancellationToken cancellationToken = default)
        {
            var timeFromDateTime = (DateTimeOffset?)command.TimeFrom;

            while (timeFromDateTime is not null)
            {
                var gamePullRecordReply = await _gameInterfaceAwcRecordServiceGrpc.GamePullRecordByAwcSpadeAsync(
                    command.GameRecordApiUrl,
                    command.GameProviderAgentId,
                    command.GameProviderCert,
                    command.TimeFrom,
                    cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                    DateTime.UtcNow.ToString("O"),
                    command,
                    gamePullRecordReply);

                if (gamePullRecordReply.Items is not null && gamePullRecordReply.Items.Any())
                {
                    await _pullRecordCommandService.ExecuteAsync(new GamePullRecordCommand(
                      GameProviderId: command.GameProviderId,
                      GameProviderCode: command.GameProviderCode,
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
