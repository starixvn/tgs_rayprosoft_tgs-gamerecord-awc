using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    internal class GameRecordMessageService : ICommandService<GameRecordMessage>
    {
        private readonly GameRecordInfoOptions _gameRecordInfoOptions;
        private readonly ILogger<GameRecordMessageService> _logger;
        private readonly IMessageSender _messageSender;

        public GameRecordMessageService(
            IOptions<GameRecordInfoOptions> options,
            IMessageSender messageSender,
            ILogger<GameRecordMessageService> logger)
        {
            _gameRecordInfoOptions = options.Value;
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ValueTask ExecuteAsync(GameRecordMessage command, CancellationToken cancellationToken = default)
        {
            var GameProviderCodeSplit = command.GameProviderCode.ToLower().Split('_');
            var subject = $"{_gameRecordInfoOptions.MongoGlobPatternPrefix}.{GameProviderCodeSplit[1]}";

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, command);

            return _messageSender.PublishAsync(subject, command.ToBson(), cancellationToken);
        }
    }
}
