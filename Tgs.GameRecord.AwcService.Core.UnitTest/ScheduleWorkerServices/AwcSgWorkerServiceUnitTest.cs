using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.ScheduleWorkerServices;

[TestClass]
public class AwcSgWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AwcSgWorkerService>.Instance;
        var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcSgCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_SG,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gamePullRecordReply = new GameInterfaceAwcServiceGamePullRecordReply();
        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .GamePullRecordByAwcSgAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>()).Returns(gamePullRecordReply);

        var service = new AwcSgWorkerService(
            fakeGameInterfaceAwcRecordServiceGrpc,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .Received(1)
            .GamePullRecordByAwcSgAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }

    [TestMethod]
    public async Task Start_Time_Greater_Than_End_Time()
    {
        var fakeLogger = NullLogger<AwcSgWorkerService>.Instance;
        var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcSgCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_SG,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddSeconds(1),
            EndTime: DateTime.UtcNow);

        var service = new AwcSgWorkerService(
            fakeGameInterfaceAwcRecordServiceGrpc,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .Received(0)
            .GamePullRecordByAwcSgAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }
}
