using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    internal class GaemSettleCommandService : ICommandService<GaemSettleCommand>
    {
        private readonly ICommandService<GameRoundCommand> _service;
        private readonly IUtensil _utensil;

        public GaemSettleCommandService(
            ICommandService<GameRoundCommand> service,
            IUtensil utensil)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(GaemSettleCommand command, CancellationToken cancellationToken = default)
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
                if (status == GameRoundStatus.Closed)
                {
                    externalClosedOn = DateTimeOffset.ParseExact(record.TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
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
}
