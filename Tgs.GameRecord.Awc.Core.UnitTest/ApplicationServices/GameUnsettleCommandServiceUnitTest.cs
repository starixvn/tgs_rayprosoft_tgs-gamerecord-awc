using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices;

public class GameUnsettleCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<GameUnsettleCommandService>.Instance;
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new GameUnsettleCommand(
            GameProviderCode: "AWC_RCB",
            Records: new GameUnsettleRecord[]{
                new GameUnsettleRecord   {
                    ExternalRoundId= "SV388-TXN2132715755",
                    Platform= "SV388",
                    PlatformTxId= "TXN2132715755",
                    GameType="LIVE",
                    GameCode= "SV-LIVE-001",
                    GameName= "CockFight_PHI",
                    RoundId="1414501",
                    UserId= "4ujxhodsopwo",
                    Currency= "HKD",
                    BetTime= "2023-09-12T04:03:55Z",
                    BetType= "WALA",
                    BetAmount= 16,
                    RealBetAmount=16,
                    GameInfo="{\"arenaCode\":\"PO1\",\"odds\":\"0.91\",\"matchno\":33,\"location\":\"PHI\",\"realBetAmount\":-16.0,\"live\":0,\"matchId\":\"1414501\",\"eventDate\":\"2023-09-11T16:00:00.000+0000\",\"result\":1,\"settled\":1,\"matchResult\":\"WALA\",\"ip\":\"182.239.88.185\",\"oddsMode\":0,\"arena\":\"Pacto De Sangre Coliseum\",\"validBet1v1\":16.0,\"bddType\":0,\"status\":\"WIN\"}"
            }});

        var service = new GameUnsettleCommandService(
            fakeLogger,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            playerCode: Arg.Is(command.Records.First().UserId),
            wagerState: Arg.Is(WagerStates.Running),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Records.First().ExternalRoundId),
            externalWagerId: Arg.Is(command.Records.First().ExternalRoundId),
            externalGameId: Arg.Is(command.Records.First().GameCode),
            externalCreatedOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalClosedOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalPayoffOn: Arg.Is(DateTimeOffset.UnixEpoch),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(command.Records.First().BetAmount.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Records.First().RealBetAmount.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.0m),
            winLoss: Arg.Is(0.0m),
            additionalData: Arg.Any<string>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
