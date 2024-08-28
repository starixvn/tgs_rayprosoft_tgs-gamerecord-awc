using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ApplicationServices.AwcSpade;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices.AwcSpade;

public class AwcSpadeManualCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<AwcSpadeManualCommandService>.Instance;
		var fakeClient = Substitute.For<IGameInterfaceAwcRecordClient>();
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new AwcSpadeManualCommand(
			ApiUrl: new("http://localhost/"),
			GameProviderCode: "AWC_SPADE",
			AgentId: "testAgentId",
			Cert: "testCert",
			StartTime: DateTime.UtcNow.AddMinutes(-1),
			EndTime: DateTime.UtcNow);

		var sut = new AwcSpadeManualCommandService(
			fakeLogger,
			fakeClient,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeClient
			.Received(1)
			.GameSettleByAwcSpadeAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			agentId: Arg.Is(command.AgentId),
			cert: Arg.Is(command.Cert),
			startTime: Arg.Any<DateTimeOffset>(),
			endTime: Arg.Any<DateTimeOffset>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
