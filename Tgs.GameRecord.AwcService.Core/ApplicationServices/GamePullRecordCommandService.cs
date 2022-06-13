using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
    {
        private readonly ICommandService<GameRoundCommand> _service;
        private readonly IUtensil _utensil;

        public GamePullRecordCommandService(
            ICommandService<GameRoundCommand> service,
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
                var playerModel = await _utensil.GetPlayerAsync(record.UserId).ConfigureAwait(false);
                var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, record.GameCode).ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, command.GameProviderId).ConfigureAwait(false);
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

                var gameRoundCommand = new GameRoundCommand(
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
                    SettleAmount: settleAmount,
                    Commission: 0,
                    Turnover: turnover,
                    PlayerWinLoss: playerWinLoss,
                    AdditionalData: JsonSerializer.Serialize(record),
                    ClientStatus: clientStatus,
                    Success: success);

                await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
