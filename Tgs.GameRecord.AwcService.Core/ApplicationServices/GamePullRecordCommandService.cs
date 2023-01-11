using System.Text.Json;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly ICommandService<GameRecordMessage> _service;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ICommandService<GameRecordMessage> service,
        IUtensil utensil)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
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
                    cancellationToken: cancellationToken).ConfigureAwait(false);
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
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }
            var status = _utensil.GetGameRoundStatus(record.TxStatus, record.SettleStatus);
            var clientStatus = _utensil.GetGameRoundClientStatus(status, record.RealWinAmount);

            var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

            DateTimeOffset? externalClosedOn = null;
            var betAmount = record.BetAmount;
            var validBetAmount = record.RealBetAmount;
            var settleAmount = default(decimal?);
            var turnover = default(decimal?);
            var playerWinLoss = default(decimal?);
            if (status == GameRoundStatus.Closed)
            {
                externalClosedOn = DateTimeOffset.ParseExact(record.TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                settleAmount = record.RealWinAmount;
                turnover = record.Turnover;
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

            var gameRoundCommand = new GameRecordMessage(
                ExternalRoundId: record.ExternalRoundId,
                GameProviderId: command.GameProviderId,
                GameProviderCode: command.GameProviderCode,
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.GameCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                Status: status,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                ExternalCreatedOn: externalCreatedOn,
                ExternalClosedOn: externalClosedOn,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                BetAmount: betAmount,
                ValidBetAmount: validBetAmount,
                SettleAmount: settleAmount.GetValueOrDefault(0),
                Commission: 0,
                Turnover: turnover.GetValueOrDefault(0),
                PlayerWinLoss: playerWinLoss.GetValueOrDefault(0),
                AdditionalData: JsonSerializer.Serialize(additionalData),
                ClientStatus: clientStatus,
                Success: success);

            await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
        }
    }
}
