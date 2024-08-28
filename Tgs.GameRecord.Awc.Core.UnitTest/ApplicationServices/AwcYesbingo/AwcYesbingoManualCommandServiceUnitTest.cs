using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcYesbingo;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices.AwcYesbingo;

public class AwcYesbingoManualCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<AwcYesbingoManualCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceAwcRecordClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new AwcYesbingoManualCommand(
			ApiUrl: new("http://localhost/"),
			GameProviderCode: "AWC_YESBINGO",
			AgentId: "testAgentId",
			Cert: "testCert",
			StartTime: DateTime.UtcNow.AddMinutes(-1),
			EndTime: DateTime.UtcNow);

		var sut = new AwcYesbingoManualCommandService(
			fakeLogger,
			fakeClient,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.GameSettleByAwcYesbingoAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			agentId: Arg.Is(command.AgentId),
			cert: Arg.Is(command.Cert),
			startTime: Arg.Any<DateTimeOffset>(),
			endTime: Arg.Any<DateTimeOffset>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
