using Tgs.GameRecord.Awc.Core;
using Tgs.GameRounds.Schedule.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Messages;

internal class ScheduleFormQueuedMessageHandler(
    ILogger<ScheduleFormQueuedMessageHandler> logger,
    IScheduleFormDispatcher dispatcher) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = GameRoundsScheduleFormRequest.Parser.ParseFrom(data.Span)
        ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{request}", DateTime.Now.ToString("o"), request);

        return dispatcher.ExecuteAsync(
            gameProviderCode: request.GameProviderCode,
            id: request.Id,
            cancellationToken: cancellationToken);
    }
}
