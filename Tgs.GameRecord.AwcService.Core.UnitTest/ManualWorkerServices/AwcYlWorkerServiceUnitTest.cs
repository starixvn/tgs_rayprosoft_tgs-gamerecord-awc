using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.ManualWorkerServices;

[TestClass]
public class AwcYlWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakeSettleCommandService = Substitute.For<ICommandService<GameSettleCommand>>();
        var fakeUtensil = Substitute.For<IUtensil>();

        var command = new AwcYlCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gameProviderId = Guid.NewGuid();

        _ = fakeUtensil.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_YL)).Returns(gameProviderId);

        var gameSettle = new GameSettleRecordReply();
        _ = fakeClient
            .GameSettleByAwcYlAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<DateTimeOffset>()).Returns(gameSettle);

        var service = new AwcYlWorkerService(
            fakeClient,
            fakeSettleCommandService,
            fakeUtensil);

        await service.ExecuteAsync(command).ConfigureAwait(false);
    }
}
