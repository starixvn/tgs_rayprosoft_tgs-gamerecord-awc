namespace Tgs.GameRecord.AwcService
{
    public record RedisOptions
    {
        public string ConnectionString { get; set; } = "localhost";
    }
}
