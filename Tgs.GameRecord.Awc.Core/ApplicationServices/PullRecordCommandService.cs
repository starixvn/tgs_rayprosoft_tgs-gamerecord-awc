using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class PullRecordCommandService(
    ILogger<PullRecordCommandService> logger,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var info in command.Infos)
        {
            try
            {
                await service.ExecuteAsync(
                    command: new HandleRecordCommand(
                        command.GameProviderCode,
                        info),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({info})", DateTime.Now.ToString("o"), info);
            }
        }
    }
}
