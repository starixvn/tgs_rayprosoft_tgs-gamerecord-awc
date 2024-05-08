using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
		var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

		var command = new PullRecordCommand(
			GameProviderCode: "AWC_PGS",
			Infos: new GameWagerInfo[]{
				new() {
				ExternalRoundId = "1704798817385242114",
				ExternalWagerId = "PG-1704798817385242114",
				ExternalGameId = "PG-SLOT-093",
				PlayerCode = "4ukqxcs9suno",
				IsClosed = true,
				IsCanceled = false,
				IsVoid = false,
				IsCashout = false,
				IsResettle = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-09-21T10:04:46Z").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-09-21T10:04:46Z").ToTimestamp(),
				Bet = 6.0m,
				ValidBet = 6.0m,
				Settle = 3.6m,
				Turnover = 6.0m,
				WinLoss = -2.4m,
				AdditionalData = "{\"ExternalRoundId\":\"PG-1704798817385242114\",\"Platform\":\"PG\",\"PlatformTxId\":\"1704798817385242114\",\"GameType\":\"SLOT\",\"GameCode\":\"PG-SLOT-093\",\"GameName\":\"Fortune Tiger\",\"RoundId\":\"1704798817385242114\",\"UserId\":\"4ukqxcs9suno\",\"Currency\":\"PHP\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-21T10:04:46Z\",\"TxTime\":\"2023-09-21T10:04:46Z\",\"UpdateTime\":\"2023-09-21T10:04:46Z\",\"BetType\":\"1\",\"BetAmount\":6,\"RealBetAmount\":6,\"Turnover\":6,\"WinAmount\":3.6,\"RealWinAmount\":3.6,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022freeGameTransactionId\\u0022:\\u0022\\u0022,\\u0022isAdjustment\\u0022:false,\\u0022isEndRound\\u0022:true,\\u0022bonusTransactionId\\u0022:\\u0022\\u0022,\\u0022isBuyFreeSpin\\u0022:false,\\u0022isFeature\\u0022:false}\"}"
			}});

		var service = new PullRecordCommandService(
			fakeLogger,
			fakeService);

		await service.ExecuteAsync(command);

		await fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new HandleRecordCommand(
				command.GameProviderCode,
				command.Infos.First())),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
