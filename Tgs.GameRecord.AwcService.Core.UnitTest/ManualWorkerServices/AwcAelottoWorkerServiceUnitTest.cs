using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.ManualWorkerServices;

[TestClass]
public class AwcAelottoWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameInterfaceAwcRecordServiceClient>();
        var fakeSettleCommandService = Substitute.For<ICommandService<GameSettleCommand>>();
        var fakeUnsettleCommandService = Substitute.For<ICommandService<GameUnsettleCommand>>();
        var fakeUtensil = Substitute.For<IUtensil>();

        var command = new AwcAelottoCommand(
            GameRecordApiUrl: new("http://localhost/"),
            GameProviderAgentId: "testAgentId",
            GameProviderCert: "testCert",
            StartTime: DateTime.UtcNow.AddMinutes(-1),
            EndTime: DateTime.UtcNow);

        var gameProviderId = Guid.NewGuid();

        _ = fakeUtensil.GetGameProviderIdAsync(Arg.Is("AWC_AELOTTO")).Returns(gameProviderId);

        var gameUnsettleReply = new GameUnsettleRecordReply();
        _ = fakeClient
            .GameUnsettleByAwcAeLottoAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<DateTimeOffset>()).Returns(gameUnsettleReply);

        var gameSettle = new GameSettleRecordReply();
        _ = fakeClient
            .GameSettleByAwcAeLottoAsync(
                Arg.Is(command.GameRecordApiUrl),
                Arg.Is(command.GameProviderAgentId),
                Arg.Is(command.GameProviderCert),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<DateTimeOffset>()).Returns(gameSettle);

        var service = new AwcAelottoWorkerService(
            fakeClient,
            fakeSettleCommandService,
            fakeUnsettleCommandService,
            fakeUtensil);

        await service.ExecuteAsync(command).ConfigureAwait(false);
    }
}
