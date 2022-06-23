using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameRecord.AwcService.Core.ApplicationServices;
using Tgs.GameRecord.AwcService.Core.ScheduleWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.UnitTest.ScheduleWorkerServices
{
    [TestClass]
    public class AwcSpadeWorkerServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeLogger = NullLogger<AwcSpadeWorkerService>.Instance;
            var fakeGameInterfaceAwcRecordServiceGrpc = Substitute.For<IGameInterfaceAwcRecordServiceGrpc>();
            var fakePullRecordCommandService = Substitute.For<ICommandService<GamePullRecordCommand>>();

            var command = new AwcSpadeCommand(
                GameRecordApiUrl: new("http://localhost/"),
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: GameProviderType.AWC_AELOTTO,
                GameProviderAgentId: "testAgentId",
                GameProviderCert: "testCert",
                TimeFrom: DateTime.UtcNow.AddMinutes(-1));

            var gamePullRecordReply = new GameInterfaceAwcServiceGamePullRecordReply();
            _ = fakeGameInterfaceAwcRecordServiceGrpc
                .GamePullRecordByAwcSpadeAsync(
                    Arg.Is(command.GameRecordApiUrl),
                    Arg.Is(command.GameProviderAgentId),
                    Arg.Is(command.GameProviderCert),
                    Arg.Any<DateTimeOffset>()).Returns(gamePullRecordReply);

            var service = new AwcSpadeWorkerService(
                fakeGameInterfaceAwcRecordServiceGrpc,
                fakeLogger,
                fakePullRecordCommandService);

            await service.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
