using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices;

internal class AwcVenusWorkerService : ICommandService<AwcVenusCommand>
{
    private readonly IGameInterfaceAwcRecordServiceClient _client;
    private readonly ILogger<AwcVenusWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;

    public AwcVenusWorkerService(
        IGameInterfaceAwcRecordServiceClient client,
        ILogger<AwcVenusWorkerService> logger,
        ICommandService<GamePullRecordCommand> pullRecordCommandService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
    }

    public async ValueTask ExecuteAsync(AwcVenusCommand command, CancellationToken cancellationToken = default)
    {
        var timeFromDateTime = (DateTimeOffset?)command.StartTime;

        while (timeFromDateTime is not null && command.EndTime.CompareTo(timeFromDateTime.Value) == 1)
        {
            _logger.LogInformation("TimeFrom:{timeFrom} | CompareTo:{compareTo}", timeFromDateTime.Value, command.EndTime.CompareTo(timeFromDateTime.Value));

            var gamePullRecordReply = await _client.GamePullRecordByAwcVenusAsync(
                command.GameRecordApiUrl,
                command.GameProviderAgentId,
                command.GameProviderCert,
                timeFromDateTime.Value,
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
