using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    internal class AwcFsWorkerService : ICommandService<AwcFsCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ILogger<AwcFsWorkerService> _logger;
        private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;

        public AwcFsWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ILogger<AwcFsWorkerService> logger,
            ICommandService<GamePullRecordCommand> pullRecordCommandService)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
        }

        public async ValueTask ExecuteAsync(AwcFsCommand command, CancellationToken cancellationToken = default)
        {
            var timeFromDateTime = (DateTimeOffset?)command.TimeFrom;

            while (timeFromDateTime is not null)
            {
                var gamePullRecordReply = await _gameInterfaceAwcRecordServiceGrpc.GamePullRecordByAwcFsAsync(
                    command.GameRecordApiUrl,
                    command.GameProviderAgentId,
                    command.GameProviderCert,
                    command.TimeFrom,
                    cancellationToken).ConfigureAwait(false);

                if (gamePullRecordReply.Items is not null && gamePullRecordReply.Items.Any())
                {
                    await _pullRecordCommandService.ExecuteAsync(new GamePullRecordCommand(
                      GameProviderId: command.GameProviderId,
                      GameProviderCode: command.GameProviderCode,
                      Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);

                    var item = gamePullRecordReply.Items.OrderByDescending(x => x.UpdateTime).First();

                    _logger.LogInformation("gamePullRecordReply.Items.OrderByDescending(x => x.UpdateTime).First()->{item}", item);

                    timeFromDateTime = !string.IsNullOrEmpty(item.UpdateTime)
                        ? DateTimeOffset.ParseExact(
                            item.UpdateTime,
                            @"yyyy-MM-dd\THH:mm:ssK",
                            null,
                            styles: System.Globalization.DateTimeStyles.RoundtripKind)
                        : null;
                }
                else
                {
                    timeFromDateTime = null;
                }
            }
        }
    }
}
