namespace Tgs.GameRecord.Awc.Core.ManualWorkerServices;

public record AwcCockfightCommand(
    string GameProviderCode,
    Uri ApiUrl,
    string AgentId,
    string Cert,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
