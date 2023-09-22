using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ManualWorkerServices;

public class AwcFsWorkerServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakeSettleCommandService = Substitute.For<ICommandService<GameSettleCommand>>();

        var command = new AwcFsCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderCode: "AWC_FS",
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gameSettle = new GameSettleRecordReply();
        _ = fakeClient
            .GameSettleByAwcFsAsync(
            apiUrl: Arg.Is(command.GameRecordApiUrl),
            agentId: Arg.Is(command.GameProviderAgentId),
            cert: Arg.Is(command.GameProviderCert),
            startTime: Arg.Any<DateTimeOffset>(),
            endTime: Arg.Any<DateTimeOffset>())
            .Returns(gameSettle);

        var service = new AwcFsWorkerService(
            fakeClient,
            fakeSettleCommandService);

        await service.ExecuteAsync(command);
    }
}
