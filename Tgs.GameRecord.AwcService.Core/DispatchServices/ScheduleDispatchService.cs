using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly IGameRecordScheduleFactory _factory;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly IScheduleDispatch _scheduleDispatch;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        IGameRecordScheduleFactory factory,
        IScheduleDispatch scheduleDispatch
        )
    {
        _gameRecordInfoOptions = options.Value;
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"GameRecord:AwcService:{command.GameProviderCode}:{command.GameProviderAgentId}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    var startTime = endTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinutes * -1);

                    await _factory.ExecuteAsync(new GameRecordScheduleCommand(
                        GameRecordApiUrl: command.GameRecordApiUrl,
                        GameProviderId: command.GameProviderId,
                        GameProviderCode: command.GameProviderCode,
                        GameProviderAgentId: command.GameProviderAgentId,
                        GameProviderCert: command.GameProviderCert,
                        StartTime: startTime), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _ = await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
