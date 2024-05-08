using Tgs.GameRecord.Awc.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core;

internal class GameRecordManualDispatcher(
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
	ICommandService<AwcYesbingoCommand> awcYesbingoWorkerService,
	ICommandService<AwcBgCommand> awcBgWorkerService) : IGameRecordManualDispatcher
{
	public ValueTask ExecuteAsync(GameRecordManualCommand command, CancellationToken cancellationToken = default)
		=> command.GameProviderCode.ToUpper() switch
		{
			"AWC_AELOTTO" => awcAelottoWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_AWS" => awcAwsWorkerservice.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_COCKFIGHT" => awcCockfightWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_FACAI" => awcFacaiWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_FS" => awcFsWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_KM" => awcKmWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_LUDO" => awcLudoWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_PGS" => awcPgsWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_RCB" => awcRcbCommand.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_SG" => awcSgCommand.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_SPADE" => awcSpadeCommand.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_VENUS" => awcVenusWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_YL" => awcYlWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_YESBINGO" => awcYesbingoWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_BG" => awcBgWorkerService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			_ => throw new NotSupportedException()
		};
}
