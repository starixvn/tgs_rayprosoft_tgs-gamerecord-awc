using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.ManualWork.Client;
using Tgs.GameRounds.WorkService.Client;
using Tgs.GameRounds.WorkService.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GameSettleCommandService : ICommandService<GameSettleCommand>
{
    private readonly IGameRoundsManualWorkClient _gameRoundsManualWorkClient;
    private readonly IGameRoundsWorkServiceClient _gameRoundsWorkServiceClient;
    private readonly ILogger<GameSettleCommandService> _logger;
    private readonly IUtensil _utensil;

    public GameSettleCommandService(
        ILogger<GameSettleCommandService> logger,
        IGameRoundsManualWorkClient gameRoundsManualWorkClient,
        IGameRoundsWorkServiceClient gameRoundsWorkServiceClient,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRoundsManualWorkClient = gameRoundsManualWorkClient ?? throw new ArgumentNullException(nameof(gameRoundsManualWorkClient));
        _gameRoundsWorkServiceClient = gameRoundsWorkServiceClient ?? throw new ArgumentNullException(nameof(gameRoundsWorkServiceClient));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GameSettleCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                bool success = true;
                var playerModel = await _utensil
                    .GetPlayerAsync(
                        playerCode: record.UserId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var gameId = await _utensil
                    .GetGameIdAsync(
                        gameProviderId: command.GameProviderId,
                        gameExternalId: record.GameCode,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _utensil
                        .GetBrandGameProviderConfigurationIdAsync(
                            brandId: playerModel.BrandId,
                            gameProviderId: command.GameProviderId,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                    if (!gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }
                }
                var status = _utensil.GetGameRoundStatus(record.TxStatus, record.SettleStatus);
                var clientStatus = _utensil.GetGameRoundClientStatus(status, record.RealWinAmount);

                var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

                DateTimeOffset? externalClosedOn = null;
                var betAmount = record.BetAmount.ToDecimal().GetValueOrDefault(0);
                var validBetAmount = record.RealBetAmount.ToDecimal().GetValueOrDefault(0);
                var settleAmount = 0.0m;
                var turnover = 0.0m;
                var playerWinLoss = 0.0m;
                if (status == RoundBiddingStatus.Closed)
                {
                    externalClosedOn = DateTimeOffset.ParseExact(record.TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    settleAmount = record.RealWinAmount.ToDecimal().GetValueOrDefault(0);
                    turnover = record.Turnover.ToDecimal().GetValueOrDefault(0);
                    playerWinLoss = settleAmount - validBetAmount;
                }

                var additionalData = new
                {
                    record.ExternalRoundId,
                    record.Platform,
                    record.PlatformTxId,
                    record.GameType,
                    record.GameCode,
                    record.GameName,
                    record.RoundId,
                    record.UserId,
                    record.Currency,
                    record.TxStatus,
                    record.SettleStatus,
                    record.BetTime,
                    record.TxTime,
                    record.UpdateTime,
                    record.BetType,
                    BetAmount = record.BetAmount.ToDecimal(),
                    RealBetAmount = record.RealBetAmount.ToDecimal(),
                    Turnover = record.Turnover.ToDecimal(),
                    WinAmount = record.WinAmount.ToDecimal(),
                    RealWinAmount = record.RealWinAmount.ToDecimal(),
                    JackpotBetAmount = record.JackpotBetAmount.ToDecimal(),
                    JackpotWinAmount = record.JackpotWinAmount.ToDecimal(),
                    record.GameInfo
                };

                if (success)
                {
                    await _gameRoundsManualWorkClient
                    .RapidCheckingAsync(
                        gameProviderId: command.GameProviderId,
                        gameProviderCode: command.GameProviderCode,
                        externalRoundId: record.ExternalRoundId,
                        gameId: gameId!.Value,
                        externalGameId: record.GameCode,
                        playerCode: playerModel!.PlayerCode,
                        walletId: playerModel!.DefaultWalletId,
                        currencyCode: playerModel!.DefaultWalletCurrencyCode,
                        brandId: playerModel!.BrandId,
                        gameProviderConfigurationId: gameProviderConfigurationId!.Value,
                        status: (GameRounds.ManualWork.Common.Protos.RoundBiddingStatus)status,
                        clientStatus: (GameRounds.ManualWork.Common.Protos.RoundBiddingClientStatus)clientStatus,
                        externalCreatedOn: externalCreatedOn,
                        externalClosedOn: externalClosedOn,
                        bet: betAmount,
                        validBet: validBetAmount,
                        settle: settleAmount,
                        commission: 0,
                        turnover: turnover,
                        playerWinLoss: playerWinLoss,
                        additionalData: JsonSerializer.Serialize(additionalData),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                }
                else
                {
                    await _gameRoundsWorkServiceClient
                    .RoundMissing(
                        gameProviderId: command.GameProviderId,
                        gameProviderCode: command.GameProviderCode,
                        externalRoundId: record.ExternalRoundId,
                        gameId: gameId ?? -1,
                        externalGameId: record.GameCode,
                        playerCode: playerModel?.PlayerCode ?? string.Empty,
                        data: JsonSerializer.Serialize(additionalData),
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "遊戲結算處理發生例外.({record})", record);
            }
        }
    }
}
