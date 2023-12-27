using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Awc.Common;

public record AwcGameRecordScheduleMessage
{
    [Required]
    public string GameProviderCode { get; init; } = default!;
    [Required]
    [DataType(DataType.Url)]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string AgentId { get; init; } = default!;
    [Required]
    public string Cert { get; init; } = default!;
    [Required]
    [DataType(DataType.DateTime)]
    public DateTimeOffset StartTime { get; init; } = default!;
}
