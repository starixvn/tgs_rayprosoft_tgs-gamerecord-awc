using Tgs.GameRecord.Awc.Core;
namespace Tgs.GameRecord.Awc;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
