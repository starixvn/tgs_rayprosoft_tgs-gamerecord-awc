using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Awc.Common;
using Tgs.GameRecord.Awc.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Messages;

internal class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	IGameRecordScheduleDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<AwcGameRecordScheduleMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Information Request:{message}",
			DateTime.Now.ToString("O"),
			subject,
			message);

		var command = new GameRecordScheduleCommand(
			GameProviderCode: message.GameProviderCode.ToUpper(),
			ApiUrl: message.ApiUrl,
			AgentId: message.AgentId,
			Cert: message.Cert,
			StartTime: message.StartTime);

		return dispatcher.ExecuteAsync(
			command: command,
			cancellationToken: cancellationToken);
	}
}
