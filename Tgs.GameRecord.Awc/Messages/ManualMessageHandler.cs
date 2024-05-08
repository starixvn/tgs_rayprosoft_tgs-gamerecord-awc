using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Awc.Common;
using Tgs.GameRecord.Awc.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Messages;

internal class ManualMessageHandler(
	ILogger<ManualMessageHandler> logger,
	IGameRecordManualDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<AwcGameRecordManualMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Information Request:{message}",
			DateTime.Now.ToString("o"),
			subject,
			message);

		var command = new GameRecordManualCommand(
			GameProviderCode: message.GameProviderCode.ToUpper(),
			ApiUrl: message.ApiUrl,
			AgentId: message.AgentId,
			Cert: message.Cert,
			StartTime: message.StartTime,
			EndTime: message.EndTime);

		return dispatcher.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
