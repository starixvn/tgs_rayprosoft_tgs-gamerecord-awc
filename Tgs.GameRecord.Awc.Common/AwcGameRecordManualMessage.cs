using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Awc.Common;

public record AwcGameRecordManualMessage
{
	[Required]
	[DefaultValue(typeof(string), "AWC_PGS")]
	public string GameProviderCode { get; init; } = default!;
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://tttint.onlinegames22.com/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "cnygpipgs")]
	public string AgentId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "te2LfPbKlL4f4rQTEBV")]
	public string Cert { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-08-27T16:30:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(DateTimeOffset), "2024-08-27T16:35:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
