using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcFacai;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices.AwcFacai;

public class AwcFacaiScheduleCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<AwcFacaiScheduleCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceAwcRecordClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new AwcFacaiScheduleCommand(
			ApiUrl: new("http://localhost/"),
			GameProviderCode: "AWC_FACAI",
			AgentId: "testAgentId",
			Cert: "testCert",
			StartTime: DateTime.UtcNow.AddMinutes(-1));

		var sut = new AwcFacaiScheduleCommandService(
			fakeLogger,
			fakeClient,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.GamePullRecordByAwcFacaiAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			agentId: Arg.Is(command.AgentId),
			cert: Arg.Is(command.Cert),
			time_from: Arg.Any<DateTimeOffset>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
