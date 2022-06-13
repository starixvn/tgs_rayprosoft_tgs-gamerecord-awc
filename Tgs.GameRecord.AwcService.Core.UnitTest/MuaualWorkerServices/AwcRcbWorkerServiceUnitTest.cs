using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.GameRecord.AwcService.Core.MuaualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.MuaualWorkerServices
{
    [TestClass]
    public class AwcRcbWorkerServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
            var fakeSettleCommandService = Substitute.For<ICommandService<GaemSettleCommand>>();
            var fakeUnsettleCommandService = Substitute.For<ICommandService<GaemUnsettleCommand>>();
            var fakeUtensil = Substitute.For<IUtensil>();

            var command = new AwcRcbCommand(
                GameRecordApiUrl: new("http://localhost/"),
                GameProviderAgentId: "testAgentId",
                GameProviderCert: "testCert",
                StartTime: DateTime.UtcNow.AddMinutes(-1),
                EndTime: DateTime.UtcNow);

            var gameProviderId = Guid.NewGuid();

            _ = fakeUtensil.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_RCB)).Returns(gameProviderId);

            var gameUnsettleReply = new GameInterfaceAwcServiceGameUnsettleRecordReply();
            _ = fakeGameInterfaceAwcRecordServiceGrpc
                .GameUnsettleByAwcRcbAsync(
                    Arg.Is(command.GameRecordApiUrl),
                    Arg.Is(command.GameProviderAgentId),
                    Arg.Is(command.GameProviderCert),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>()).Returns(gameUnsettleReply);

            var gameSettle = new GameInterfaceAwcServiceGameSettleRecordReply();
            _ = fakeGameInterfaceAwcRecordServiceGrpc
                .GameSettleByAwcRcbAsync(
                    Arg.Is(command.GameRecordApiUrl),
                    Arg.Is(command.GameProviderAgentId),
                    Arg.Is(command.GameProviderCert),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>()).Returns(gameSettle);

            var service = new AwcRcbWorkerService(
                fakeGameInterfaceAwcRecordServiceGrpc,
                fakeSettleCommandService,
                fakeUnsettleCommandService,
                fakeUtensil);

            await service.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
