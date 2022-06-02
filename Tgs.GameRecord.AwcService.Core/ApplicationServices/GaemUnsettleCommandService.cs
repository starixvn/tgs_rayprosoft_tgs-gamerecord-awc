using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    internal class GaemUnsettleCommandService : ICommandService<GaemUnsettleCommand>
    {
        private readonly ICommandService<GameRoundCommand> _service;
        private readonly IUtensil _utensil;

        public GaemUnsettleCommandService(
            ICommandService<GameRoundCommand> service,
            IUtensil utensil)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(GaemUnsettleCommand command, CancellationToken cancellationToken = default)
        {
            foreach (var transaction in command.Transactions)
            {
                bool success = true;
                var playerModel = await _utensil.GetPlayerAsync(transaction.UserId).ConfigureAwait(false);
                var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, transaction.GameCode).ConfigureAwait(false);
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

                var externalCreatedOn = DateTimeOffset.ParseExact(transaction.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

                var gameRoundCommand = new GameRoundCommand(
                    ExternalRoundId: transaction.ExternalRoundId,
                    GameProviderId: command.GameProviderId,
                    GameProviderCode: command.GameProviderCode,
                    PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId: gameId ?? -1,
                    ExternalGameId: transaction.GameCode,
                    CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                    WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    ExternalCreatedOn: externalCreatedOn,
                    ExternalClosedOn: null,
                    BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    BetAmount: transaction.BetAmount,
                    ValidBetAmount: transaction.RealBetAmount,
                    SettleAmount: null,
                    Commission: 0,
                    Turnover: null,
                    PlayerWinLoss: null,
                    AdditionalData: JsonSerializer.Serialize(transaction),
                    ClientStatus: GameRoundClientStatus.Open,
                    Success: success);

                await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
