using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Awc.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ManualWorkerServices;

public class AwcAwsWorkerserviceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakeSettleCommandService = Substitute.For<ICommandService<GameSettleCommand>>();

        var command = new AwcAwsCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderCode: "AWC_PGS",
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gameSettle = new GameSettleRecordReply();
        _ = fakeClient
            .GameSettleByAwcAwsAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<DateTimeOffset>()).Returns(gameSettle);

        var service = new AwcAwsWorkerservice(
            fakeClient,
            fakeSettleCommandService);

        await service.ExecuteAsync(command);
    }
}
