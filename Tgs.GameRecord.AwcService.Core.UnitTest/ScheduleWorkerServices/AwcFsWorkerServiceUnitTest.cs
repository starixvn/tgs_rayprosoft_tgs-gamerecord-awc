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
public class AwcFsWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<AwcFsWorkerService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcFsCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_FS,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gamePullRecordReply = new GamePullRecordReply();
        _ = fakeClient
            .GamePullRecordByAwcFsAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>()).Returns(gamePullRecordReply);

        var service = new AwcFsWorkerService(
            fakeClient,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .GamePullRecordByAwcFsAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }

    [TestMethod]
    public async Task Start_Time_Greater_Than_End_Time()
    {
        var fakeLogger = NullLogger<AwcFsWorkerService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

        var command = new AwcFsCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AWC_FS,
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddSeconds(1),
            EndTime: DateTime.UtcNow);

        var service = new AwcFsWorkerService(
            fakeClient,
            fakeLogger,
            fakePullRecordCommandService);

        await service.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeClient
            .Received(0)
            .GamePullRecordByAwcFsAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>());
    }
}
