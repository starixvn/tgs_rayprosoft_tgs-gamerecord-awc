using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices
{
    internal class AwcFacaiWorkerService : ICommandService<AwcFacaiCommand>
    {
        private readonly IGameInterfaceAwcRecordServiceGrpc _gameInterfaceAwcRecordServiceGrpc;
        private readonly ILogger<AwcFacaiWorkerService> _logger;
        private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;

        public AwcFacaiWorkerService(
            IGameInterfaceAwcRecordServiceGrpc gameInterfaceAwcRecordServiceGrpc,
            ILogger<AwcFacaiWorkerService> logger,
            ICommandService<GamePullRecordCommand> pullRecordCommandService)
        {
            _gameInterfaceAwcRecordServiceGrpc = gameInterfaceAwcRecordServiceGrpc ?? throw new ArgumentNullException(nameof(gameInterfaceAwcRecordServiceGrpc));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
        }

        public async ValueTask ExecuteAsync(AwcFacaiCommand command, CancellationToken cancellationToken = default)
        {
            var timeFromDateTime = (DateTimeOffset?)command.TimeFrom;

            while (timeFromDateTime is not null)
            {
                var gamePullRecordReply = await _gameInterfaceAwcRecordServiceGrpc.GamePullRecordByAwcFacaiAsync(
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

                    try
                    {
                        var item = gamePullRecordReply.Items.OrderByDescending(x => x.UpdateTime).First();

                        if (!string.IsNullOrEmpty(item.UpdateTime))

                            timeFromDateTime = DateTimeOffset.ParseExact(
                                item.UpdateTime,
                                @"yyyy-MM-dd\THH:mm:ssK",
                                null,
                                styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Items:{items}", gamePullRecordReply.Items);
                        timeFromDateTime = null;
                    }
                }
                else
                {
                    timeFromDateTime = null;
                }
            }
        }
    }
}
