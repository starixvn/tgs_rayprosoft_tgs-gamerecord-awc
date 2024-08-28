using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAelotto;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAws;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcBg;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcCockfight;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFacai;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFs;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcKm;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcLudo;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPgs;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcRcb;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSg;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSpade;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcVenus;

using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYesbingo;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYl;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class SchedulePullRecordCommandService(
	ICommandService<AwcAelottoScheduleCommand> awcAelottoScheduleCommandService,
	ICommandService<AwcAwsScheduleCommand> awcAwsScheduleCommandService,
	ICommandService<AwcCockfightScheduleCommand> awcCockfightScheduleCommandService,
	ICommandService<AwcFacaiScheduleCommand> awcFacaiScheduleCommandService,
	ICommandService<AwcFsScheduleCommand> awcFsScheduleCommandService,
	ICommandService<AwcKmScheduleCommand> awcKmScheduleCommandService,
	ICommandService<AwcLudoScheduleCommand> awcLudoScheduleCommandService,
	ICommandService<AwcPgsScheduleCommand> awcPgsScheduleCommandService,
	ICommandService<AwcRcbScheduleCommand> awcRcbScheduleCommandService,
	ICommandService<AwcSgScheduleCommand> awcSgScheduleCommandService,
	ICommandService<AwcSpadeScheduleCommand> awcSpadeScheduleCommandService,
	ICommandService<AwcVenusScheduleCommand> awcVenusScheduleCommandService,
	ICommandService<AwcYlScheduleCommand> awcYlScheduleCommandService,
	ICommandService<AwcYesbingoScheduleCommand> awcYesbingoScheduleCommandService,
	ICommandService<AwcBgScheduleCommand> awcBgScheduleCommandService) : ICommandService<SchedulePullRecordCommand>
{
	public ValueTask ExecuteAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default)
			=> command.GameProviderCode.ToUpper() switch
			{
				"AWC_AELOTTO" => awcAelottoScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_AWS" => awcAwsScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_COCKFIGHT" => awcCockfightScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_FACAI" => awcFacaiScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_FS" => awcFsScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_KM" => awcKmScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_LUDO" => awcLudoScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_PGS" => awcPgsScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_RCB" => awcRcbScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_SG" => awcSgScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_SPADE" => awcSpadeScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_VENUS" => awcVenusScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_YL" => awcYlScheduleCommandService.ExecuteAsync(
					 command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_YESBINGO" => awcYesbingoScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				"AWC_BG" => awcBgScheduleCommandService.ExecuteAsync(
					command: new(
						GameProviderCode: command.GameProviderCode,
						ApiUrl: command.ApiUrl,
						AgentId: command.AgentId,
						Cert: command.Cert,
						StartTime: command.StartTime),
					cancellationToken),
				_ => throw new NotSupportedException()
			};
}
