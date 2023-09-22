namespace Tgs.GameRecord.Awc.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
