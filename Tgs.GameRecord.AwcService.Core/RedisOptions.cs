namespace Tgs.GameRecord.AwcService.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
