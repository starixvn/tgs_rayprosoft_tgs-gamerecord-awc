using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices;

public class GamePullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Lose()
    {
        var fakeLogger = NullLogger<GamePullRecordCommandService>.Instance;
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new GamePullRecordCommand(
            GameProviderCode: "AWC_PGS",
            Records: new GamePullRecord[]{
                new() {
                    ExternalRoundId= "PG-1704798847479330816",
                    Platform= "PG",
                    PlatformTxId= "1704798847479330816",
                    GameType="SLOT",
                    GameCode="PG-SLOT-112",
                    GameName= "Fortune Rabbit",
                    RoundId="1704798847479330816",
                    UserId= "4ukg5be8jk05",
                    Currency= "PHP",
                    TxStatus= 1,
                    SettleStatus=0,
                    BetTime="2023-09-21T10:04:53Z",
                    TxTime= "2023-09-21T10:04:53Z",
                    UpdateTime="2023-09-21T10:04:53Z",
                    BetType="1",
                    BetAmount=3m,
                    RealBetAmount=3m,
                    Turnover=3m,
                    WinAmount= 0m,
                    RealWinAmount= 0m,
                    JackpotBetAmount= 0m,
                    JackpotWinAmount= 0m,
                    GameInfo = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":true,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}",
                    AdditionalData="{\"ExternalRoundId\":\"PG-1704798847479330816\",\"Platform\":\"PG\",\"PlatformTxId\":\"1704798847479330816\",\"GameType\":\"SLOT\",\"GameCode\":\"PG-SLOT-112\",\"GameName\":\"Fortune Rabbit\",\"RoundId\":\"1704798847479330816\",\"UserId\":\"4ukg5be8jk05\",\"Currency\":\"PHP\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-21T10:04:53Z\",\"TxTime\":\"2023-09-21T10:04:53Z\",\"UpdateTime\":\"2023-09-21T10:04:53Z\",\"BetType\":\"1\",\"BetAmount\":3,\"RealBetAmount\":3,\"Turnover\":3,\"WinAmount\":0,\"RealWinAmount\":0,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022freeGameTransactionId\\u0022:\\u0022\\u0022,\\u0022isAdjustment\\u0022:false,\\u0022isEndRound\\u0022:true,\\u0022bonusTransactionId\\u0022:\\u0022\\u0022,\\u0022isBuyFreeSpin\\u0022:false,\\u0022isFeature\\u0022:false}\"}"
            }});

        _ = fakeUtensil
            .IsClosed(
            txStatus: Arg.Is(command.Records.First().TxStatus))
            .Returns(true);
        _ = fakeUtensil
            .IsVoid(
            txStatus: Arg.Is(command.Records.First().TxStatus),
            settleStatus: Arg.Is(command.Records.First().SettleStatus))
            .Returns(false);
        _ = fakeUtensil
            .GetWagerStates(
            isVoid: Arg.Is(false),
            realWinAmount: command.Records.First().RealWinAmount.ToDecimal().GetValueOrDefault(0))
            .Returns(WagerStates.Lose);
        var service = new GamePullRecordCommandService(
            fakeLogger,
            fakeUtensil,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            playerCode: Arg.Is(command.Records.First().UserId),
            wagerState: Arg.Is(WagerStates.Lose),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Records.First().RoundId),
            externalWagerId: Arg.Is(command.Records.First().ExternalRoundId),
            externalGameId: Arg.Is(command.Records.First().GameCode),
            externalCreatedOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalClosedOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalPayoffOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(3.0m),
            validBet: Arg.Is(3.0m),
            settle: Arg.Is(0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(3.0m),
            winLoss: Arg.Is(-3.0m),
            additionalData: Arg.Any<JsonNode>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Running()
    {
        var fakeLogger = NullLogger<GamePullRecordCommandService>.Instance;
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new GamePullRecordCommand(
            GameProviderCode: "AWC_RCB",
            Records: new GamePullRecord[]{
                new() {
                    ExternalRoundId= "SV388-TXN2132715755",
                    Platform= "SV388",
                    PlatformTxId= "TXN2132715755",
                    GameType="LIVE",
                    GameCode= "SV-LIVE-001",
                    GameName= "CockFight_PHI",
                    RoundId="1414501",
                    UserId= "4ujxhodsopwo",
                    Currency= "HKD",
                    TxStatus= 0,
                    SettleStatus=0,
                    BetTime= "2023-09-12T04:03:55Z",
                    BetType= "WALA",
                    BetAmount= 16,
                    RealBetAmount=16,
                    Turnover= 0,
                    WinAmount=  0,
                    RealWinAmount= 0,
                    JackpotBetAmount= 0,
                    JackpotWinAmount= 0,
                    GameInfo="{\"arenaCode\":\"PO1\",\"odds\":\"0.91\",\"matchno\":33,\"location\":\"PHI\",\"realBetAmount\":-16.0,\"live\":0,\"matchId\":\"1414501\",\"eventDate\":\"2023-09-11T16:00:00.000+0000\",\"result\":1,\"settled\":1,\"matchResult\":\"WALA\",\"ip\":\"182.239.88.185\",\"oddsMode\":0,\"arena\":\"Pacto De Sangre Coliseum\",\"validBet1v1\":16.0,\"bddType\":0,\"status\":\"WIN\"}",
                    AdditionalData="{\"ExternalRoundId\":\"SV388-TXN2132715755\",\"Platform\":\"SV388\",\"PlatformTxId\":\"TXN2132715755\",\"GameType\":\"LIVE\",\"GameCode\":\"SV-LIVE-001\",\"GameName\":\"CockFight_PHI\",\"RoundId\":\"1414501\",\"UserId\":\"4ujxhodsopwo\",\"Currency\":\"HKD\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-12T04:03:55Z\",\"TxTime\":\"2023-09-12T04:06:22Z\",\"UpdateTime\":\"2023-09-12T04:06:31Z\",\"BetType\":\"WALA\",\"BetAmount\":16,\"RealBetAmount\":16,\"Turnover\":16,\"WinAmount\":30.56,\"RealWinAmount\":30.56,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022arenaCode\\u0022:\\u0022PO1\\u0022,\\u0022odds\\u0022:\\u00220.91\\u0022,\\u0022matchno\\u0022:33,\\u0022location\\u0022:\\u0022PHI\\u0022,\\u0022realBetAmount\\u0022:-16.0,\\u0022live\\u0022:0,\\u0022matchId\\u0022:\\u00221414501\\u0022,\\u0022eventDate\\u0022:\\u00222023-09-11T16:00:00.000\\u002B0000\\u0022,\\u0022result\\u0022:1,\\u0022settled\\u0022:1,\\u0022matchResult\\u0022:\\u0022WALA\\u0022,\\u0022ip\\u0022:\\u0022182.239.88.185\\u0022,\\u0022oddsMode\\u0022:0,\\u0022arena\\u0022:\\u0022Pacto De Sangre Coliseum\\u0022,\\u0022validBet1v1\\u0022:16.0,\\u0022bddType\\u0022:0,\\u0022status\\u0022:\\u0022WIN\\u0022}\"}"
            }});

        _ = fakeUtensil
            .IsClosed(
            txStatus: Arg.Is(command.Records.First().TxStatus))
            .Returns(false);
        _ = fakeUtensil
            .IsVoid(
            txStatus: Arg.Is(command.Records.First().TxStatus),
            settleStatus: Arg.Is(command.Records.First().SettleStatus))
            .Returns(false);

        var service = new GamePullRecordCommandService(
            fakeLogger,
            fakeUtensil,
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
            externalRoundId: Arg.Is(command.Records.First().RoundId),
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
            additionalData: Arg.Any<JsonNode>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Win()
    {
        var fakeLogger = NullLogger<GamePullRecordCommandService>.Instance;
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new GamePullRecordCommand(
            GameProviderCode: "AWC_PGS",
            Records: new GamePullRecord[]{
                new() {
                    ExternalRoundId= "PG-1704798817385242114",
                    Platform= "PG",
                    PlatformTxId= "1704798817385242114",
                    GameType="SLOT",
                    GameCode= "PG-SLOT-093",
                    GameName= "Fortune Tiger",
                    RoundId="1704798817385242114",
                    UserId= "4ukqxcs9suno",
                    Currency= "PHP",
                    TxStatus= 1,
                    SettleStatus=0,
                    BetTime= "2023-09-21T10:04:46Z",
                    TxTime= "2023-09-21T10:04:46Z",
                    UpdateTime= "2023-09-21T10:04:46Z",
                    BetType="1",
                    BetAmount=6m,
                    RealBetAmount=6m,
                    Turnover=6m,
                    WinAmount= 3.6m,
                    RealWinAmount= 3.6m,
                    JackpotBetAmount= 0,
                    JackpotWinAmount= 0,
                    GameInfo ="{\"arenaCode\":\"PO1\",\"odds\":\"0.91\",\"matchno\":33,\"location\":\"PHI\",\"realBetAmount\":-16.0,\"live\":0,\"matchId\":\"1414501\",\"eventDate\":\"2023-09-11T16:00:00.000+0000\",\"result\":1,\"settled\":1,\"matchResult\":\"WALA\",\"ip\":\"182.239.88.185\",\"oddsMode\":0,\"arena\":\"Pacto De Sangre Coliseum\",\"validBet1v1\":16.0,\"bddType\":0,\"status\":\"WIN\"}",
                    AdditionalData="{\"ExternalRoundId\":\"PG-1704798817385242114\",\"Platform\":\"PG\",\"PlatformTxId\":\"1704798817385242114\",\"GameType\":\"SLOT\",\"GameCode\":\"PG-SLOT-093\",\"GameName\":\"Fortune Tiger\",\"RoundId\":\"1704798817385242114\",\"UserId\":\"4ukqxcs9suno\",\"Currency\":\"PHP\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-21T10:04:46Z\",\"TxTime\":\"2023-09-21T10:04:46Z\",\"UpdateTime\":\"2023-09-21T10:04:46Z\",\"BetType\":\"1\",\"BetAmount\":6,\"RealBetAmount\":6,\"Turnover\":6,\"WinAmount\":3.6,\"RealWinAmount\":3.6,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022freeGameTransactionId\\u0022:\\u0022\\u0022,\\u0022isAdjustment\\u0022:false,\\u0022isEndRound\\u0022:true,\\u0022bonusTransactionId\\u0022:\\u0022\\u0022,\\u0022isBuyFreeSpin\\u0022:false,\\u0022isFeature\\u0022:false}\"}"
            }});

        _ = fakeUtensil
            .IsClosed(
            txStatus: Arg.Is(command.Records.First().TxStatus))
            .Returns(true);
        _ = fakeUtensil
            .IsVoid(
            txStatus: Arg.Is(command.Records.First().TxStatus),
            settleStatus: Arg.Is(command.Records.First().SettleStatus))
            .Returns(false);
        _ = fakeUtensil
            .GetWagerStates(
            isVoid: Arg.Is(false),
            realWinAmount: command.Records.First().RealWinAmount.ToDecimal().GetValueOrDefault(0))
            .Returns(WagerStates.Win);
        var service = new GamePullRecordCommandService(
            fakeLogger,
            fakeUtensil,
            fakeClient);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            playerCode: Arg.Is(command.Records.First().UserId),
            wagerState: Arg.Is(WagerStates.Win),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Records.First().RoundId),
            externalWagerId: Arg.Is(command.Records.First().ExternalRoundId),
            externalGameId: Arg.Is(command.Records.First().GameCode),
            externalCreatedOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalClosedOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalPayoffOn: Arg.Is(DateTimeOffset.ParseExact(command.Records.First().TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind)),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(6m),
            validBet: Arg.Is(6m),
            settle: Arg.Is(3.6m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(6m),
            winLoss: Arg.Is(-2.4m),
            additionalData: Arg.Any<JsonNode>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
