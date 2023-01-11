using System.Text.Json;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GameSettleCommandService : ICommandService<GameSettleCommand>
{
    private readonly ICommandService<GameRecordMessage> _service;
    private readonly IUtensil _utensil;

    public GameSettleCommandService(
        ICommandService<GameRecordMessage> service,
        IUtensil utensil)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GameSettleCommand command, CancellationToken cancellationToken = default)
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
            if (status == GameRoundStatus.Closed)
            {
                externalClosedOn = DateTimeOffset.ParseExact(record.TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
            }
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
                BetAmount: record.BetAmount,
                ValidBetAmount: record.RealBetAmount,
                SettleAmount: record.RealWinAmount,
                Commission: 0,
                Turnover: record.Turnover,
                PlayerWinLoss: record.RealWinAmount - record.RealBetAmount,
                AdditionalData: JsonSerializer.Serialize(record),
                ClientStatus: clientStatus,
                Success: success);

            await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
        }
    }
}
