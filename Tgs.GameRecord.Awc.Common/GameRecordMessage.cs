namespace Tgs.GameRecord.Awc.Common;

public record GameRecordMessage
{
    public Uri ApiUrl { get; init; } = default!;
    public string AgentId { get; init; } = default!;
    public string Cert { get; init; } = default!;
    public DateTimeOffset StartTime { get; init; } = default!;
    public DateTimeOffset EndTime { get; init; } = default!;
}
