using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    internal class GameRoundCommandService : ICommandService<GameRoundCommand>
    {
        private readonly ILogger<GameRoundCommandService> _logger;
        private readonly IMessageSender _messageSender;
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;

        public GameRoundCommandService(
            IOptions<GameRecordInfoOptions> options,
            IMessageSender messageSender,
            ILogger<GameRoundCommandService> logger)
        {
            _gameRecordInfoOptions = options.Value;
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask ExecuteAsync(GameRoundCommand command, CancellationToken cancellationToken = default)
        {
            var GameProviderCodeSplit = command.GameProviderCode.ToLower().Split('_');
            var subject = $"{_gameRecordInfoOptions.MongoGlobPatternPrefix}.{GameProviderCodeSplit[1]}";

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, command);

            return _messageSender.PublishAsync(subject, command.ToBson(), cancellationToken);
        }
    }
}
