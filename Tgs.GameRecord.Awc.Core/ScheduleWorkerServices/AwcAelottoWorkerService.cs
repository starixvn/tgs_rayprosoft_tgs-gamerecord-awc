using Microsoft.Extensions.Logging;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;

internal class AwcAelottoWorkerService : ICommandService<AwcAelottoCommand>
{
    private readonly IGameInterfaceAwcRecordServiceClient _client;
    private readonly ILogger<AwcAelottoWorkerService> _logger;
    private readonly ICommandService<GamePullRecordCommand> _pullRecordCommandService;

    public AwcAelottoWorkerService(
        IGameInterfaceAwcRecordServiceClient client,
        ILogger<AwcAelottoWorkerService> logger,
        ICommandService<GamePullRecordCommand> pullRecordCommandService)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pullRecordCommandService = pullRecordCommandService ?? throw new ArgumentNullException(nameof(pullRecordCommandService));
    }

    public async ValueTask ExecuteAsync(AwcAelottoCommand command, CancellationToken cancellationToken = default)
    {
        var gamePullRecordReply = await _client.GamePullRecordByAwcAeLottoAsync(
            apiUrl: command.GameRecordApiUrl,
            agentId: command.GameProviderAgentId,
            cert: command.GameProviderCert,
            time_from: command.StartTime,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gamePullRecordReply is null)
        {
            _logger.LogError("LogOn:{logOn} Message: This Reply is null.({command})", DateTime.Now.ToString("o"), command);
            return;
        }

        var records = gamePullRecordReply.Items ?? Enumerable.Empty<GamePullRecord>();

        if (!records.Any())
        {
            _logger.LogInformation("LogOn:{logOn} Message: The records are empty.({command})", DateTime.Now.ToString("o"), command);
            return;
        }

        var recordCommand = new GamePullRecordCommand(
            GameProviderCode: command.GameProviderCode,
            Records: records);

        await _pullRecordCommandService.ExecuteAsync(
            command: recordCommand,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
