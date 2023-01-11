namespace Tgs.GameRecord.AwcService.Core;

public record NatsSettingOptions
{
    public string Group { get; set; } = default!;
    public string Subject { get; set; } = default!;
}
