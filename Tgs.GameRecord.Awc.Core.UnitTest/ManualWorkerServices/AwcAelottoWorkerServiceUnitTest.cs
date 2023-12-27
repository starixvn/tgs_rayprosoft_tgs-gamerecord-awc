using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ManualWorkerServices;

public class AwcAelottoWorkerServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordClient>();
        var fakeService = Substitute.For<ICommandService<PullRecordCommand>>();

        var command = new AwcAelottoCommand(
            ApiUrl: new("http://localhost/"),
            GameProviderCode: "AWC_AELOTTO",
            AgentId: "testAgentId",
            Cert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var service = new AwcAelottoWorkerService(
            fakeClient,
            fakeService);

        await service.ExecuteAsync(command);

        _ = fakeClient
            .Received(1)
            .GameUnsettleByAwcAeLottoAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            agentId: Arg.Is(command.AgentId),
            cert: Arg.Is(command.Cert),
            startTime: Arg.Any<DateTimeOffset>(),
            endTime: Arg.Any<DateTimeOffset>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeClient
            .Received(1)
            .GameSettleByAwcAeLottoAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            agentId: Arg.Is(command.AgentId),
            cert: Arg.Is(command.Cert),
            startTime: Arg.Any<DateTimeOffset>(),
            endTime: Arg.Any<DateTimeOffset>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
