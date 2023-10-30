using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Awc.Common;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.JobApplicationServices;

internal class JobExecuteCommandService : ICommandService<JobExecuteCommand>
{
    private readonly IGameRoundsScheduleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly double _intervalMinutes;
    private readonly ILogger<JobExecuteCommandService> _logger;

    public JobExecuteCommandService(
        ILogger<JobExecuteCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IGameRoundsScheduleClient client)
    {
        _gameRecordInfo = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _intervalMinutes = _gameRecordInfo.IntervalMinutesMax < _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMax
            : _gameRecordInfo.IntervalMinutesMinimum > _gameRecordInfo.IntervalMinutes
            ? _gameRecordInfo.IntervalMinutesMinimum
            : _gameRecordInfo.IntervalMinutes;
    }

    public ValueTask ExecuteAsync(JobExecuteCommand command, CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
        var startTime = endTime.AddMinutes(_intervalMinutes * -1);

        var parameters = new GameRecordMessage
        {
            ApiUrl = command.ApiUrl,
            AgentId = command.AgentId,
            Cert = command.Cert,
            StartTime = startTime,
            EndTime = endTime
        };

        _logger.LogDebug("LogOn:{logOn} Parameters:{parameters}", DateTime.Now.ToString("u"), parameters);

        return _client
            .ScheduleFormQueuedAsync(
            gameProviderCode: command.GameProviderCode,
            name: command.Name,
            subject: "gp.gamerecord.scheduleform.awc.queued",
            source: ScheduleFormSource.Schedule,
            parameters: JsonSerializer.Serialize(parameters),
            cancellationToken: cancellationToken);
    }
}
