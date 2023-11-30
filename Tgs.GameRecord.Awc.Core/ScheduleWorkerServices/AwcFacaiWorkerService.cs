using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

internal class AwcFacaiWorkerService(
    IGameInterfaceAwcRecordServiceClient client,
    ILogger<AwcFacaiWorkerService> logger,
    ICommandService<GamePullRecordCommand> pullRecordCommandService) : ICommandService<AwcFacaiCommand>
{
    public async ValueTask ExecuteAsync(AwcFacaiCommand command, CancellationToken cancellationToken = default)
    {
        var gamePullRecordReply = await client.GamePullRecordByAwcFacaiAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            time_from: command.StartTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gamePullRecordReply is null)
        {
            logger.LogError("LogOn:{logOn} Message: This Reply is null.({command})", DateTime.Now.ToString("o"), command);
            return;
        }

        var records = gamePullRecordReply.Items ?? Enumerable.Empty<GamePullRecord>();

        if (!records.Any())
        {
            logger.LogInformation("LogOn:{logOn} Message: The records are empty.({command})", DateTime.Now.ToString("o"), command);
            return;
        }

        var recordCommand = new GamePullRecordCommand(
            GameProviderCode: command.GameProviderCode,
            Records: records);

        await pullRecordCommandService.ExecuteAsync(
            command: recordCommand,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
