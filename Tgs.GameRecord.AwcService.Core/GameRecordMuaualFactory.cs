using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.MuaualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core
{
    internal class GameRecordMuaualFactory : IGameRecordMuaualFactory
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
        private readonly ICommandService<AwcYlCommand> _awcYlWorkerService;

        public GameRecordMuaualFactory(
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
            ICommandService<AwcYlCommand> awcYlWorkerService

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
        }

        public ValueTask ExecuteAsync(string gameProviderCode, GameRecordMuaualCommand command, CancellationToken cancellationToken = default)
        {
            return gameProviderCode switch
            {
                var code when code == GameProviderType.AWC_AELOTTO => _awcAelottoWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_AWS => _awcAwsWorkerservice.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_COCKFIGHT => _awcCockfightWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_FACAI => _awcFacaiWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_FS => _awcFsWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_KM => _awcKmWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_LUDO => _awcLudoWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_PGS => _awcPgsWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_RCB => _awcRcbCommand.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_SG => _awcSgCommand.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_SPADE => _awcSpadeCommand.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_VENUS => _awcVenusWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                var code when code == GameProviderType.AWC_YL => _awcYlWorkerService.ExecuteAsync(new(
                                                                        GameRecordApiUrl: command.GameRecordApiUrl,
                                                                        GameProviderAgentId: command.GameProviderAgentId,
                                                                        GameProviderCert: command.GameProviderCert,
                                                                        StartTime: command.StartTime,
                                                                        EndTime: command.EndTime), cancellationToken),
                _ => throw new NotSupportedException()
            };
        }
    }
}
