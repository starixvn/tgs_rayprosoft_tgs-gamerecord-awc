using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ScheduleWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ScheduleWorkerServices;

public class AwcKmWorkerServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AwcKmWorkerService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordClient>();
        var fakeService = Substitute.For<ICommandService<PullRecordCommand>>();

        var command = new AwcKmCommand(
            ApiUrl: new("http://localhost/"),
            GameProviderCode: "AWC_KM",
            AgentId: "testAgentId",
            Cert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1));

        var service = new AwcKmWorkerService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service.ExecuteAsync(command);

        _ = fakeClient
            .Received(1)
            .GamePullRecordByAwcKmAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            agentId: Arg.Is(command.AgentId),
            cert: Arg.Is(command.Cert),
            time_from: Arg.Any<DateTimeOffset>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
