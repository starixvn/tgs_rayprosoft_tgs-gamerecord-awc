using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.DispatchServices
{
    internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
    {
        private readonly IGameRecordFactory _factory;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;

        public ScheduleDispatchService(
            IOptions<GameRecordInfoOptions> options,
            IGameRecordFactory factory)
        {
            _gameRecordInfoOptions = options.Value;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
        {
            var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

            var startTime = endTime.AddMinutes((double)_gameRecordInfoOptions.IntervalMinute * -1);

            var gameRecordCommand = new GameRecordCommand(
                GameRecordApiUrl: command.GameRecordApiUrl,
                GameProviderAgentId: command.GameProviderAgentId,
                GameProviderCert: command.GameProviderCert,
                StartTime: startTime,
                EndTime: endTime);

            return _factory.ExecuteAsync(command.GameProviderCode, gameRecordCommand, cancellationToken: cancellationToken);
        }
    }
}
