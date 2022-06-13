using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.ManualWorkerServices
{
    [TestClass]
    public class AwcVenusWorkerServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
            var fakeSettleCommandService = Substitute.For<ICommandService<GaemSettleCommand>>();
            var fakeUtensil = Substitute.For<IUtensil>();

            var command = new AwcVenusCommand(
                GameRecordApiUrl: new("http://localhost/"),
                GameProviderAgentId: "testAgentId",
                GameProviderCert: "testCert",
                StartTime: DateTime.UtcNow.AddMinutes(-1),
                EndTime: DateTime.UtcNow);

            var gameProviderId = Guid.NewGuid();

            _ = fakeUtensil.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_VENUS)).Returns(gameProviderId);

            var gameSettle = new GameInterfaceAwcServiceGameSettleRecordReply();
            _ = fakeGameInterfaceAwcRecordServiceGrpc
                .GameSettleByAwcVenusAsync(
                    Arg.Is(command.GameRecordApiUrl),
                    Arg.Is(command.GameProviderAgentId),
                    Arg.Is(command.GameProviderCert),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>()).Returns(gameSettle);

            var service = new AwcVenusWorkerService(
                fakeGameInterfaceAwcRecordServiceGrpc,
                fakeSettleCommandService,
                fakeUtensil);

            await service.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
