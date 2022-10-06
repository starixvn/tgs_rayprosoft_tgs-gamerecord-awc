using System.Text;
using System.Text.Json;
using Tgs.GameRecord.AwcService.Core;
using Tgs.MessageQueue;

namespace Tgs.GameRecord.AwcManualService
{
    public class ManualMessageHandler : IMessageProcessor
    {
        private readonly IGameRecordManualFactory _factory;

        public ManualMessageHandler(IGameRecordManualFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            var json = Encoding.UTF8.GetString(data.Span);
            var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

            var gameRecordCommand = new GameRecordManualCommand(
                GameRecordApiUrl: request.GameRecordApiUrl,
                GameProviderAgentId: request.GameProviderAgentId,
                GameProviderCert: request.GameProviderCert,
                StartTime: request.StartTime,
                EndTime: request.EndTime);

            await _factory.ExecuteAsync(request.GameProviderCode, command: gameRecordCommand, cancellationToken: cancellationToken).ConfigureAwait(false);

            var response = new ManualMessageResponse(true);
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        }
    }
}
