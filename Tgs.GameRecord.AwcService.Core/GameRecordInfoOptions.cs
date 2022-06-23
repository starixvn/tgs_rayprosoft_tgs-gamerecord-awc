namespace Tgs.GameRecord.AwcService.Core
{
    public record GameRecordInfoOptions
    {
        public string GameProviderCode { get; set; } = default!;
        public decimal IntervalMinute { get; set; } = 1;
    }
}
