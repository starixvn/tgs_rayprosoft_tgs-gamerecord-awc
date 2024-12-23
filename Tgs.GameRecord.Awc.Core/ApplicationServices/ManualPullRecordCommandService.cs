using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAelotto;

using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcAws;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcBg;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcCockfight;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFacai;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFs;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcKm;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcLudo;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPgs;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcPinnacle;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcRcb;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSg;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSpade;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcVenus;

using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYesbingo;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYl;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class ManualPullRecordCommandService(
	ICommandService<AwcAelottoManualCommand> awcAelottoManualCommandService,
	ICommandService<AwcAwsManualCommand> awcAwsManualCommandService,
	ICommandService<AwcCockfightManualCommand> awcCockfightManualCommandService,
	ICommandService<AwcFacaiManualCommand> awcFacaiManualCommandService,
	ICommandService<AwcFsManualCommand> awcFsManualCommandService,
	ICommandService<AwcKmManualCommand> awcKmManualCommandService,
	ICommandService<AwcLudoManualCommand> awcLudoManualCommandService,
	ICommandService<AwcPgsManualCommand> awcPgsManualCommandService,
	ICommandService<AwcRcbManualCommand> awcRcbManualCommandService,
	ICommandService<AwcSgManualCommand> awcSgManualCommandService,
	ICommandService<AwcSpadeManualCommand> awcSpadeManualCommandService,
	ICommandService<AwcVenusManualCommand> awcVenusManualCommandService,
	ICommandService<AwcYlManualCommand> awcYlManualCommandService,
	ICommandService<AwcYesbingoManualCommand> awcYesbingoManualCommandService,
	ICommandService<AwcBgManualCommand> awcBgManualCommandService,
	ICommandService<AwcPinnacleManualCommand> awcPinnacleManualCommandService) : ICommandService<ManualPullRecordCommand>
{
	public ValueTask ExecuteAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default)
		=> command.GameProviderCode.ToUpper() switch
		{
			"AWC_AELOTTO" => awcAelottoManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_AWS" => awcAwsManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_COCKFIGHT" => awcCockfightManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_FACAI" => awcFacaiManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_FS" => awcFsManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_KM" => awcKmManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_LUDO" => awcLudoManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_PGS" => awcPgsManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_RCB" => awcRcbManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_SG" => awcSgManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_SPADE" => awcSpadeManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_VENUS" => awcVenusManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_YL" => awcYlManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_YESBINGO" => awcYesbingoManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_BG" => awcBgManualCommandService.ExecuteAsync(
				command: new(
					GameProviderCode: command.GameProviderCode,
					ApiUrl: command.ApiUrl,
					AgentId: command.AgentId,
					Cert: command.Cert,
					StartTime: command.StartTime,
					EndTime: command.EndTime),
				cancellationToken: cancellationToken),
			"AWC_PINNACLE" => awcPinnacleManualCommandService.ExecuteAsync(
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
