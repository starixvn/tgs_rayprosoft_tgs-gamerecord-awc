using Microsoft.Extensions.Options;
using RedLockNet;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.DispatchServices
{
    internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
    {
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly IGameRecordScheduleFactory _factory;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;

        public ScheduleDispatchService(
            IOptions<GameRecordInfoOptions> options,
            IGameRecordScheduleFactory factory,
            IDistributedLockFactory distributedLockFactory)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            _gameRecordInfoOptions = options.Value;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
        }

        public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
        {
            var resource = $"{command.GameProviderCode}:{command.GameProviderAgentId}".ToLower();
            var expiryTime = TimeSpan.FromSeconds(120);
            var waitTime = TimeSpan.FromSeconds(30);
            var retryTime = TimeSpan.FromSeconds(10);

            using (var redLock = await _distributedLockFactory.CreateLockAsync(
                resource: resource,
                expiryTime: expiryTime,
                waitTime: waitTime,
                retryTime: retryTime,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                if (redLock.IsAcquired)
                {
                    var nowTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    var timeFrom = nowTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinute * -1);

                    await _factory.ExecuteAsync(new GameRecordScheduleCommand(
                        GameRecordApiUrl: command.GameRecordApiUrl,
                        GameProviderId: command.GameProviderId,
                        GameProviderCode: command.GameProviderCode,
                        GameProviderAgentId: command.GameProviderAgentId,
                        GameProviderCert: command.GameProviderCert,
                        TimeFrom: timeFrom), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
