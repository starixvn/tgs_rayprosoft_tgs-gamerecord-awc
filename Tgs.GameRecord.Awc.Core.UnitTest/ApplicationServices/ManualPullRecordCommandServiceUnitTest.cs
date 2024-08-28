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

public class ManualPullRecordCommandServiceUnitTest
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
		var fakeAwcAelottoManualCommandService = Substitute.For<ICommandService<AwcAelottoManualCommand>>();
		var fakeAwcAwsManualCommandService = Substitute.For<ICommandService<AwcAwsManualCommand>>();
		var fakeAwcCockfightManualCommandService = Substitute.For<ICommandService<AwcCockfightManualCommand>>();
		var fakeAwcFacaiManualCommandService = Substitute.For<ICommandService<AwcFacaiManualCommand>>();
		var fakeAwcFsManualCommandService = Substitute.For<ICommandService<AwcFsManualCommand>>();
		var fakeAwcKmManualCommandService = Substitute.For<ICommandService<AwcKmManualCommand>>();
		var fakeAwcLudoManualCommandService = Substitute.For<ICommandService<AwcLudoManualCommand>>();
		var fakeAwcPgsManualCommandService = Substitute.For<ICommandService<AwcPgsManualCommand>>();
		var fakeAwcRcbManualCommandService = Substitute.For<ICommandService<AwcRcbManualCommand>>();
		var fakeAwcSgManualCommandService = Substitute.For<ICommandService<AwcSgManualCommand>>();
		var fakeAwcSpadeManualCommandService = Substitute.For<ICommandService<AwcSpadeManualCommand>>();
		var fakeAwcVenusManualCommandService = Substitute.For<ICommandService<AwcVenusManualCommand>>();
		var fakeAwcYlManualCommandService = Substitute.For<ICommandService<AwcYlManualCommand>>();
		var fakeAwcYesbingoManualCommandService = Substitute.For<ICommandService<AwcYesbingoManualCommand>>();
		var fakeAwcBgManualCommandService = Substitute.For<ICommandService<AwcBgManualCommand>>();

		var command = new ManualPullRecordCommand(
			ApiUrl: new("http://localhost/"),
			GameProviderCode: game_provider_code,
			AgentId: "testAgentId",
			Cert: "testCert",
			StartTime: DateTime.UtcNow.AddMinutes(-1),
			EndTime: DateTime.UtcNow);

		var sut = new ManualPullRecordCommandService(
			fakeAwcAelottoManualCommandService,
			fakeAwcAwsManualCommandService,
			fakeAwcCockfightManualCommandService,
			fakeAwcFacaiManualCommandService,
			fakeAwcFsManualCommandService,
			fakeAwcKmManualCommandService,
			fakeAwcLudoManualCommandService,
			fakeAwcPgsManualCommandService,
			fakeAwcRcbManualCommandService,
			fakeAwcSgManualCommandService,
			fakeAwcSpadeManualCommandService,
			fakeAwcVenusManualCommandService,
			fakeAwcYlManualCommandService,
			fakeAwcYesbingoManualCommandService,
			fakeAwcBgManualCommandService);

		await sut.ExecuteAsync(command);

		_ = fakeAwcAelottoManualCommandService
			.Received(awc_aelotto)
			.ExecuteAsync(
			command: Arg.Is<AwcAelottoManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcAwsManualCommandService
			.Received(awc_aws)
			.ExecuteAsync(
			command: Arg.Is<AwcAwsManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcCockfightManualCommandService
			.Received(awc_cockfight)
			.ExecuteAsync(
			command: Arg.Is<AwcCockfightManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcFacaiManualCommandService
			.Received(awc_facai)
			.ExecuteAsync(
			command: Arg.Is<AwcFacaiManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcFsManualCommandService
			.Received(awc_fs)
			.ExecuteAsync(
			command: Arg.Is<AwcFsManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcKmManualCommandService
			.Received(awc_km)
			.ExecuteAsync(
			command: Arg.Is<AwcKmManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcLudoManualCommandService
			.Received(awc_ludo)
			.ExecuteAsync(
			command: Arg.Is<AwcLudoManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcPgsManualCommandService
			.Received(awc_pgs)
			.ExecuteAsync(
			command: Arg.Is<AwcPgsManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcRcbManualCommandService
			.Received(awc_rcb)
			.ExecuteAsync(
			command: Arg.Is<AwcRcbManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcSgManualCommandService
			.Received(awc_sg)
			.ExecuteAsync(
			command: Arg.Is<AwcSgManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcSpadeManualCommandService
			.Received(awc_spade)
			.ExecuteAsync(
			command: Arg.Is<AwcSpadeManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcVenusManualCommandService
			.Received(awc_venus)
			.ExecuteAsync(
			command: Arg.Is<AwcVenusManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcYlManualCommandService
			.Received(awc_yl)
			.ExecuteAsync(
			command: Arg.Is<AwcYlManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcYesbingoManualCommandService
			.Received(awc_yesbingo)
			.ExecuteAsync(
			command: Arg.Is<AwcYesbingoManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeAwcBgManualCommandService
			.Received(awc_bg)
			.ExecuteAsync(
			command: Arg.Is<AwcBgManualCommand>(
				compare => compare.GameProviderCode == command.GameProviderCode &&
				compare.ApiUrl == command.ApiUrl &&
				compare.AgentId == command.AgentId &&
				compare.Cert == command.Cert &&
				compare.StartTime == command.StartTime &&
				compare.EndTime == command.EndTime),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
