using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.DispatchServices
{
    internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
    {
        private readonly IGameRecordScheduleFactory _factory;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;

        public ScheduleDispatchService(
            IOptions<GameRecordInfoOptions> options,
            IGameRecordScheduleFactory factory)
        {
            _gameRecordInfoOptions = options.Value;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
        {
            var nowTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
            var timeFrom = nowTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinute * -1);

            return _factory.ExecuteAsync(new GameRecordScheduleCommand(
                GameRecordApiUrl: command.GameRecordApiUrl,
                GameProviderId: command.GameProviderId,
                GameProviderCode: command.GameProviderCode,
                GameProviderAgentId: command.GameProviderAgentId,
                GameProviderCert: command.GameProviderCert,
                TimeFrom: timeFrom), cancellationToken: cancellationToken);
        }
    }
}
