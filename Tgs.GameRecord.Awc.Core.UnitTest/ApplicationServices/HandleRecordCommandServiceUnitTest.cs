using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Awc.Protos;
using Tgs.GameRecord.Awc.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Client;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Awc.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			GameProviderCode: "AWC_PGS",
			Info: new GameWagerInfo
			{
				ExternalRoundId = "1704798847479330816",
				ExternalWagerId = "PG-1704798847479330816",
				ExternalGameId = "PG-SLOT-112",
				PlayerCode = "4ukg5be8jk05",
				IsClosed = true,
				IsCanceled = false,
				IsVoid = false,
				IsCashout = false,
				IsResettle = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-09-21T10:04:53Z").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-09-21T10:04:53Z").ToTimestamp(),
				Bet = 3.0m,
				ValidBet = 3.0m,
				Settle = 0.0m,
				Turnover = 3.0m,
				WinLoss = -3.0m,
				AdditionalData = "{\"ExternalRoundId\":\"PG-1704798847479330816\",\"Platform\":\"PG\",\"PlatformTxId\":\"1704798847479330816\",\"GameType\":\"SLOT\",\"GameCode\":\"PG-SLOT-112\",\"GameName\":\"Fortune Rabbit\",\"RoundId\":\"1704798847479330816\",\"UserId\":\"4ukg5be8jk05\",\"Currency\":\"PHP\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-21T10:04:53Z\",\"TxTime\":\"2023-09-21T10:04:53Z\",\"UpdateTime\":\"2023-09-21T10:04:53Z\",\"BetType\":\"1\",\"BetAmount\":3,\"RealBetAmount\":3,\"Turnover\":3,\"WinAmount\":0,\"RealWinAmount\":0,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022freeGameTransactionId\\u0022:\\u0022\\u0022,\\u0022isAdjustment\\u0022:false,\\u0022isEndRound\\u0022:true,\\u0022bonusTransactionId\\u0022:\\u0022\\u0022,\\u0022isBuyFreeSpin\\u0022:false,\\u0022isFeature\\u0022:false}\"}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = command.Info.ExternalGameId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Loss,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 3.0m,
				ValidBet = 3.0m,
				Settle = 0.0M,
				Commission = 0.0m,
				Turnover = 3.0m,
				WinLoss = -3.0m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Running()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			GameProviderCode: "AWC_RCB",
			Info: new GameWagerInfo
			{
				ExternalRoundId = "1414501",
				ExternalWagerId = "SV388-TXN2132715755",
				ExternalGameId = "SV-LIVE-001",
				PlayerCode = "4ujxhodsopwo",
				IsClosed = false,
				IsCanceled = false,
				IsVoid = false,
				IsCashout = false,
				IsResettle = false,
				ExternalCreatedOn = DateTimeOffset.Parse("2023-09-12T04:03:55Z").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
				Bet = 16m,
				ValidBet = 16m,
				Settle = 0.0m,
				Turnover = 0.0m,
				WinLoss = 0.0m,
				AdditionalData = "{\"ExternalRoundId\":\"SV388-TXN2132715755\",\"Platform\":\"SV388\",\"PlatformTxId\":\"TXN2132715755\",\"GameType\":\"LIVE\",\"GameCode\":\"SV-LIVE-001\",\"GameName\":\"CockFight_PHI\",\"RoundId\":\"1414501\",\"UserId\":\"4ujxhodsopwo\",\"Currency\":\"HKD\",\"TxStatus\":1,\"SettleStatus\":0,\"BetTime\":\"2023-09-12T04:03:55Z\",\"TxTime\":\"2023-09-12T04:06:22Z\",\"UpdateTime\":\"2023-09-12T04:06:31Z\",\"BetType\":\"WALA\",\"BetAmount\":16,\"RealBetAmount\":16,\"Turnover\":16,\"WinAmount\":30.56,\"RealWinAmount\":30.56,\"JackpotBetAmount\":0,\"JackpotWinAmount\":0,\"GameInfo\":\"{\\u0022arenaCode\\u0022:\\u0022PO1\\u0022,\\u0022odds\\u0022:\\u00220.91\\u0022,\\u0022matchno\\u0022:33,\\u0022location\\u0022:\\u0022PHI\\u0022,\\u0022realBetAmount\\u0022:-16.0,\\u0022live\\u0022:0,\\u0022matchId\\u0022:\\u00221414501\\u0022,\\u0022eventDate\\u0022:\\u00222023-09-11T16:00:00.000\\u002B0000\\u0022,\\u0022result\\u0022:1,\\u0022settled\\u0022:1,\\u0022matchResult\\u0022:\\u0022WALA\\u0022,\\u0022ip\\u0022:\\u0022182.239.88.185\\u0022,\\u0022oddsMode\\u0022:0,\\u0022arena\\u0022:\\u0022Pacto De Sangre Coliseum\\u0022,\\u0022validBet1v1\\u0022:16.0,\\u0022bddType\\u0022:0,\\u0022status\\u0022:\\u0022WIN\\u0022}\"}"
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = command.Info.ExternalGameId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = DateTimeOffset.UnixEpoch,
				ExternalPayoffOn = DateTimeOffset.UnixEpoch,
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Running,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 16.0m,
				ValidBet = 16.0m,
				Settle = 0.0M,
				Commission = 0.0m,
				Turnover = 0.0M,
				WinLoss = 0.0M,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var faceLogger = NullLogger<HandleRecordCommandService>.Instance;
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeGameRecordHandleClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			GameProviderCode: "AWC_RCB",
			Info: new GameWagerInfo
			{
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
			});

		var sut = new HandleRecordCommandService(
			faceLogger,
			fakePolicySettingsOptions,
			fakeGameRecordHandleClient);

		await sut.ExecuteAsync(command);

		_ = fakeGameRecordHandleClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is(new Handle.Common.ViewModels.GameRecordHandleViewModel
			{
				GameProviderCode = command.GameProviderCode,
				PlayerCode = command.Info.PlayerCode,
				ExternalRoundId = command.Info.ExternalRoundId,
				ExternalBetId = command.Info.ExternalWagerId,
				ExternalGameId = command.Info.ExternalGameId,
				ExternalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn = command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn = DateTimeOffset.UnixEpoch,
				BetState = Handle.Common.Protos.BetState.Win,
				BetType = Handle.Common.Protos.BetType.Normal,
				FreeGameState = Handle.Common.Protos.FreeGameState.None,
				GambleState = Handle.Common.Protos.GambleState.None,
				CashoutState = Handle.Common.Protos.CashoutState.None,
				ResettleState = Handle.Common.Protos.ResettleState.None,
				JackpotState = Handle.Common.Protos.JackpotState.None,
				Bet = 6.0m,
				ValidBet = 6.0m,
				Settle = 3.6m,
				Commission = 0.0m,
				Turnover = 6.0m,
				WinLoss = -2.4m,
				Jackpot = 0.0M,
				AdditionalData = command.Info.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
