namespace Tgs.GameRecord.AwcService.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public decimal IntervalMinutes { get; set; } = 1;
    public decimal IntervalMinutesMax { get; set; } = 60;
    public decimal IntervalMinutesMinimum { get; set; } = 1;
    public decimal LockMinute { get; set; } = 1;
}
