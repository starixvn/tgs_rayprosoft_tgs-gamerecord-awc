using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
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

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices;

public class SchedulePullRecordCommandServiceUnitTest
{
	[Theory]
	[InlineData("AWC_AELOTTO", 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_AWS", 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_COCKFIGHT", 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_FACAI", 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_FS", 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_KM", 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_LUDO", 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_PGS", 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_RCB", 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0)]
	[InlineData("AWC_SG", 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0)]
	[InlineData("AWC_SPADE", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0)]
	[InlineData("AWC_VENUS", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)]
	[InlineData("AWC_YL", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0)]
	[InlineData("AWC_YESBINGO", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0)]
	[InlineData("AWC_BG", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1)]
	public async Task ExecuteAsync(
		string game_provider_code,
		int awc_aelotto,
		int awc_aws,
		int awc_cockfight,
		int awc_facai,
		int awc_fs,
		int awc_km,
		int awc_ludo,
		int awc_pgs,
		int awc_rcb,
		int awc_sg,
		int awc_spade,
		int awc_venus,
		int awc_yl,
		int awc_yesbingo,
		int awc_bg)
	{
		var fakeAwcAelottoScheduleCommandService = Substitute.For<ICommandService<AwcAelottoScheduleCommand>>();
		var fakeAwcAwsScheduleCommandService = Substitute.For<ICommandService<AwcAwsScheduleCommand>>();
		var fakeAwcCockfightScheduleCommandService = Substitute.For<ICommandService<AwcCockfightScheduleCommand>>();
		var fakeAwcFacaiScheduleCommandService = Substitute.For<ICommandService<AwcFacaiScheduleCommand>>();
		var fakeAwcFsScheduleCommandService = Substitute.For<ICommandService<AwcFsScheduleCommand>>();
		var fakeAwcKmScheduleCommandService = Substitute.For<ICommandService<AwcKmScheduleCommand>>();
		var fakeAwcLudoScheduleCommandService = Substitute.For<ICommandService<AwcLudoScheduleCommand>>();
		var fakeAwcPgsScheduleCommandService = Substitute.For<ICommandService<AwcPgsScheduleCommand>>();
		var fakeAwcRcbScheduleCommandService = Substitute.For<ICommandService<AwcRcbScheduleCommand>>();
		var fakeAwcSgScheduleCommandService = Substitute.For<ICommandService<AwcSgScheduleCommand>>();
		var fakeAwcSpadeScheduleCommandService = Substitute.For<ICommandService<AwcSpadeScheduleCommand>>();
		var fakeAwcVenusScheduleCommandService = Substitute.For<ICommandService<AwcVenusScheduleCommand>>();
		var fakeAwcYlScheduleCommandService = Substitute.For<ICommandService<AwcYlScheduleCommand>>();
		var fakeAwcYesbingoScheduleCommandService = Substitute.For<ICommandService<AwcYesbingoScheduleCommand>>();
		var fakeAwcBgScheduleCommandService = Substitute.For<ICommandService<AwcBgScheduleCommand>>();

		var command = new SchedulePullRecordCommand(
			ApiUrl: new("http://localhost/"),
			GameProviderCode: game_provider_code,
			AgentId: "testAgentId",
			Cert: "testCert",
			StartTime: DateTime.UtcNow.AddMinutes(-1));

		var sut = new SchedulePullRecordCommandService(
			fakeAwcAelottoScheduleCommandService,
			fakeAwcAwsScheduleCommandService,
			fakeAwcCockfightScheduleCommandService,
			fakeAwcFacaiScheduleCommandService,
			fakeAwcFsScheduleCommandService,
			fakeAwcKmScheduleCommandService,
			fakeAwcLudoScheduleCommandService,
			fakeAwcPgsScheduleCommandService,
			fakeAwcRcbScheduleCommandService,
			fakeAwcSgScheduleCommandService,
			fakeAwcSpadeScheduleCommandService,
			fakeAwcVenusScheduleCommandService,
			fakeAwcYlScheduleCommandService,
			fakeAwcYesbingoScheduleCommandService,
			fakeAwcBgScheduleCommandService);

		await sut.ExecuteAsync(command);

		_ = fakeAwcAelottoScheduleCommandService
			.Received(awc_aelotto)
			.ExecuteAsync(
			command: Arg.Is<AwcAelottoScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcAwsScheduleCommandService
			.Received(awc_aws)
			.ExecuteAsync(
			command: Arg.Is<AwcAwsScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcCockfightScheduleCommandService
			.Received(awc_cockfight)
			.ExecuteAsync(
			command: Arg.Is<AwcCockfightScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcFacaiScheduleCommandService
			.Received(awc_facai)
			.ExecuteAsync(
			command: Arg.Is<AwcFacaiScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcFsScheduleCommandService
			.Received(awc_fs)
			.ExecuteAsync(
			command: Arg.Is<AwcFsScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcKmScheduleCommandService
			.Received(awc_km)
			.ExecuteAsync(
			command: Arg.Is<AwcKmScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcLudoScheduleCommandService
			.Received(awc_ludo)
			.ExecuteAsync(
			command: Arg.Is<AwcLudoScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcPgsScheduleCommandService
			.Received(awc_pgs)
			.ExecuteAsync(
			command: Arg.Is<AwcPgsScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcRcbScheduleCommandService
			.Received(awc_rcb)
			.ExecuteAsync(
			command: Arg.Is<AwcRcbScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcSgScheduleCommandService
			.Received(awc_sg)
			.ExecuteAsync(
			command: Arg.Is<AwcSgScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcSpadeScheduleCommandService
			.Received(awc_spade)
			.ExecuteAsync(
			command: Arg.Is<AwcSpadeScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcVenusScheduleCommandService
			.Received(awc_venus)
			.ExecuteAsync(
			command: Arg.Is<AwcVenusScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcYlScheduleCommandService
			.Received(awc_yl)
			.ExecuteAsync(
			command: Arg.Is<AwcYlScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcYesbingoScheduleCommandService
			.Received(awc_yesbingo)
			.ExecuteAsync(
			command: Arg.Is<AwcYesbingoScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcBgScheduleCommandService
			.Received(awc_bg)
			.ExecuteAsync(
			command: Arg.Is<AwcBgScheduleCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
