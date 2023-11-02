using Tgs.GameRecord.Awc.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core;

internal class GameRecordManualFactory : IGameRecordManualFactory
{
    private readonly ICommandService<AwcAelottoCommand> _awcAelottoWorkerService;
    private readonly ICommandService<AwcAwsCommand> _awcAwsWorkerservice;
    private readonly ICommandService<AwcCockfightCommand> _awcCockfightWorkerService;
    private readonly ICommandService<AwcFacaiCommand> _awcFacaiWorkerService;
    private readonly ICommandService<AwcFsCommand> _awcFsWorkerService;
    private readonly ICommandService<AwcKmCommand> _awcKmWorkerService;
    private readonly ICommandService<AwcLudoCommand> _awcLudoWorkerService;
    private readonly ICommandService<AwcPgsCommand> _awcPgsWorkerService;
    private readonly ICommandService<AwcRcbCommand> _awcRcbCommand;
    private readonly ICommandService<AwcSgCommand> _awcSgCommand;
    private readonly ICommandService<AwcSpadeCommand> _awcSpadeCommand;
    private readonly ICommandService<AwcVenusCommand> _awcVenusWorkerService;
    private readonly ICommandService<AwcYesbingoCommand> _awcYesbingoWorkerService;
    private readonly ICommandService<AwcYlCommand> _awcYlWorkerService;

    public GameRecordManualFactory(
        ICommandService<AwcAelottoCommand> awcAelottoWorkerService,
        ICommandService<AwcAwsCommand> awcAwsWorkerservice,
        ICommandService<AwcCockfightCommand> awcCockfightWorkerService,
        ICommandService<AwcFacaiCommand> awcFacaiWorkerService,
        ICommandService<AwcFsCommand> awcFsWorkerService,
        ICommandService<AwcKmCommand> awcKmWorkerService,
        ICommandService<AwcLudoCommand> awcLudoWorkerService,
        ICommandService<AwcPgsCommand> awcPgsWorkerService,
        ICommandService<AwcRcbCommand> awcRcbCommand,
        ICommandService<AwcSgCommand> awcSgCommand,
        ICommandService<AwcSpadeCommand> awcSpadeCommand,
        ICommandService<AwcVenusCommand> awcVenusWorkerService,
        ICommandService<AwcYlCommand> awcYlWorkerService,
        ICommandService<AwcYesbingoCommand> awcYesbingoWorkerService

        )
    {
        _awcAelottoWorkerService = awcAelottoWorkerService ?? throw new ArgumentNullException(nameof(awcAelottoWorkerService));
        _awcAwsWorkerservice = awcAwsWorkerservice ?? throw new ArgumentNullException(nameof(awcAwsWorkerservice));
        _awcCockfightWorkerService = awcCockfightWorkerService ?? throw new ArgumentNullException(nameof(awcCockfightWorkerService));
        _awcFacaiWorkerService = awcFacaiWorkerService ?? throw new ArgumentNullException(nameof(awcFacaiWorkerService));
        _awcFsWorkerService = awcFsWorkerService ?? throw new ArgumentNullException(nameof(awcFsWorkerService));
        _awcKmWorkerService = awcKmWorkerService ?? throw new ArgumentNullException(nameof(awcKmWorkerService));
        _awcLudoWorkerService = awcLudoWorkerService ?? throw new ArgumentNullException(nameof(awcLudoWorkerService));
        _awcPgsWorkerService = awcPgsWorkerService ?? throw new ArgumentNullException(nameof(awcPgsWorkerService));
        _awcRcbCommand = awcRcbCommand ?? throw new ArgumentNullException(nameof(awcRcbCommand));
        _awcSgCommand = awcSgCommand ?? throw new ArgumentNullException(nameof(awcSgCommand));
        _awcSpadeCommand = awcSpadeCommand ?? throw new ArgumentNullException(nameof(awcSpadeCommand));
        _awcVenusWorkerService = awcVenusWorkerService ?? throw new ArgumentNullException(nameof(awcVenusWorkerService));
        _awcYlWorkerService = awcYlWorkerService ?? throw new ArgumentNullException(nameof(awcYlWorkerService));
        _awcYesbingoWorkerService = awcYesbingoWorkerService ?? throw new ArgumentNullException(nameof(awcYesbingoWorkerService));
    }

    public ValueTask ExecuteAsync(GameRecordManualCommand command, CancellationToken cancellationToken = default)
        => command.GameProviderCode.ToUpper() switch
        {
            "AWC_AELOTTO" => _awcAelottoWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_AWS" => _awcAwsWorkerservice
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_COCKFIGHT" => _awcCockfightWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_FACAI" => _awcFacaiWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_FS" => _awcFsWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_KM" => _awcKmWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_LUDO" => _awcLudoWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_PGS" => _awcPgsWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_RCB" => _awcRcbCommand
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_SG" => _awcSgCommand
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_SPADE" => _awcSpadeCommand
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_VENUS" => _awcVenusWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_YL" => _awcYlWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            "AWC_YESBINGO" => _awcYesbingoWorkerService
            .ExecuteAsync(
                command: new(
                    GameRecordApiUrl: command.GameRecordApiUrl,
                    GameProviderCode: command.GameProviderCode.ToUpper(),
                    GameProviderAgentId: command.GameProviderAgentId,
                    GameProviderCert: command.GameProviderCert,
                    StartTime: command.StartTime,
                    EndTime: command.EndTime),
                cancellationToken: cancellationToken),
            _ => throw new NotSupportedException()
        };
}
