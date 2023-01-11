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
public class AwcLudoWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AwcLudoWorkerService>.Instance;
        var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcLudoCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_LUDO,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gamePullRecordReply = new GameInterfaceAwcServiceGamePullRecordReply();
        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .GamePullRecordByAwcLudoAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>()).Returns(gamePullRecordReply);

        var service = new AwcLudoWorkerService(
            fakeGameInterfaceAwcRecordServiceGrpc,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .Received(1)
            .GamePullRecordByAwcLudoAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }

    [TestMethod]
    public async Task Start_Time_Greater_Than_End_Time()
    {
        var fakeLogger = NullLogger<AwcLudoWorkerService>.Instance;
        var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcLudoCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_LUDO,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddSeconds(1),
            EndTime: DateTime.UtcNow);

        var service = new AwcLudoWorkerService(
            fakeGameInterfaceAwcRecordServiceGrpc,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeGameInterfaceAwcRecordServiceGrpc
            .Received(0)
            .GamePullRecordByAwcLudoAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }
}
