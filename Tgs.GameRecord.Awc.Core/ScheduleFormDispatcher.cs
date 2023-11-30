using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Awc.Common;
using Tgs.GameRounds.Schedule.Client;
using Tgs.GameRounds.Schedule.Protos;

namespace Tgs.GameRecord.Awc.Core;

internal class ScheduleFormDispatcher(
    ILogger<ScheduleFormDispatcher> logger,
    IGameRoundsScheduleClient clinet,
    IScheduleFormLock scheduleFormLock,
    IGameRecordScheduleFactory scheduleFactory,
    IGameRecordManualFactory manualFactory) : IScheduleFormDispatcher
{
    public async ValueTask ExecuteAsync(
        string gameProviderCode,
        long id,
        CancellationToken cancellationToken = default)
    {
        var scheduleForm = await clinet.ScheduleFormGetAsync(
            gameProviderCode: gameProviderCode,
            id: id,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"The game provider ({gameProviderCode}) cannot find the schedule form. ({id})");

        if (scheduleForm.State != ScheduleFormState.Queued)
        {
            logger.LogWarning("This game provider ({gameProviderCode}) schedule form status is not queued.({id})", gameProviderCode, id);
            return;
        }

        if (!await scheduleFormLock.LockingAsync(
            gameProviderCode: gameProviderCode,
            id: id)
            .ConfigureAwait(false))
        {
            logger.LogWarning("LogOn:{logOn} Message:This game provider ({gameProviderCode}) schedule form has been locked. ({id})",
                DateTime.Now.ToString("o"),
                gameProviderCode,
                id);
            return;
        }

        try
        {
            await clinet.ScheduleFormProcessingAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var parameter = JsonSerializer.Deserialize<GameRecordMessage>(scheduleForm.Parameters)
                ?? throw new Exception($"This game provider ({gameProviderCode}) scheduling form parameter is null. ({id})");

            if (scheduleForm.Source == ScheduleFormSource.Schedule)
            {
                var command = new GameRecordScheduleCommand(
                    GameRecordApiUrl: parameter.ApiUrl,
                    GameProviderCode: scheduleForm.GameProviderCode,
                    GameProviderAgentId: parameter.AgentId,
                    GameProviderCert: parameter.Cert,
                    StartTime: parameter.StartTime);

                logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await scheduleFactory.ExecuteAsync(
                    command: command,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var command = new GameRecordManualCommand(
                    GameRecordApiUrl: parameter.ApiUrl,
                    GameProviderCode: scheduleForm.GameProviderCode,
                    GameProviderAgentId: parameter.AgentId,
                    GameProviderCert: parameter.Cert,
                    StartTime: parameter.StartTime,
                    EndTime: parameter.EndTime);

                logger.LogInformation("LogOn:{logOn} execute the command:{command}", DateTime.Now.ToString("o"), command);

                await manualFactory.ExecuteAsync(
                    command: command,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            await clinet.ScheduleFormCompleteAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred in the game provider ({gameProviderCode}) schedule form. ({id})", gameProviderCode, id);

            await clinet.ScheduleFormFaileAsync(
                gameProviderCode: gameProviderCode,
                id: id,
                status: ex.Message,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _ = await scheduleFormLock.UnlockAsync(
                gameProviderCode: gameProviderCode,
                id: id)
                .ConfigureAwait(false);
        }
    }
}
