using System.Text;
using System.Text.Json;
using Tgs.GameRecord.Awc.Common;
using Tgs.GameRecord.Awc.Core;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Messages;

internal class ScheduleMessageHandler(
	ILogger<ScheduleMessageHandler> logger,
	ISchedulePullRecordDispatcher dispatcher) : IMessageHandler<ReadOnlyMemory<byte>>
{
	public ValueTask HandleAsync(string subject, ReadOnlyMemory<byte> data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		var json = Encoding.UTF8.GetString(data.Span);
		var message = JsonSerializer.Deserialize<AwcGameRecordScheduleMessage>(json)
			?? throw new NullReferenceException("The request data resolves to a null value");

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					GameProviderCode = message.GameProviderCode.ToUpper(),
					message.ApiUrl,
					message.AgentId,
					message.Cert,
					StartTime = message.StartTime.ToString("u")
				},
				Message = "This message receiving handler."
			}));

		return dispatcher.PushCommandAsync(
			command: new SchedulePullRecordCommand(
				GameProviderCode: message.GameProviderCode.ToUpper(),
				ApiUrl: message.ApiUrl,
				AgentId: message.AgentId,
				Cert: message.Cert,
				StartTime: message.StartTime),
			cancellationToken: cancellationToken);
	}
}
