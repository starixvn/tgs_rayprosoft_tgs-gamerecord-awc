using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Awc.Common;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Controllers;

[Route("[controller]")]
[ApiController]
public class AwcController : ControllerBase
{
	/// <summary>
	/// Games the record manual asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("manual")]
	public ValueTask GameRecordManualAsync(
		[FromServices] IMessageSender messageSender,
		AwcGameRecordManualMessage source)
		=> messageSender.PublishAsync(
			subject: AwcNatsConst.ManualPublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record schedule asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("schedule")]
	public ValueTask GameRecordScheduleAsync(
		[FromServices] IMessageSender messageSender,
		AwcGameRecordScheduleMessage source)
		=> messageSender.PublishAsync(
			subject: AwcNatsConst.SchedulePublish,
			data: JsonSerializer.SerializeToUtf8Bytes(source),
			cancellationToken: HttpContext.RequestAborted);
}
