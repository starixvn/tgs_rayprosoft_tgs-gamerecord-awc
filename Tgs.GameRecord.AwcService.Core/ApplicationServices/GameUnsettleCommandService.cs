using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GameUnsettleCommandService : ICommandService<GameUnsettleCommand>
{
    private readonly IGameRoundsWorkClient _client;
    private readonly ILogger<GameUnsettleCommandService> _logger;
    private readonly IUtensil _utensil;

    public GameUnsettleCommandService(
        ILogger<GameUnsettleCommandService> logger,
        IGameRoundsWorkClient client,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GameUnsettleCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
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

                var gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                    brandId: playerModel.BrandId,
                    gameProviderId: command.GameProviderId,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

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
                    record.BetTime,
                    record.BetType,
                    BetAmount = record.BetAmount.ToDecimal().GetValueOrDefault(0),
                    RealBetAmount = record.RealBetAmount.ToDecimal().GetValueOrDefault(0),
                    record.GameInfo
                };

                await _client
                    .RoundExchangeRateBiddingAsync(
                    gameProviderId: command.GameProviderId,
                    gameProviderCode: command.GameProviderCode,
                    externalRoundId: record.ExternalRoundId,
                    gameId: gameId,
                    externalGameId: record.GameCode,
                    playerCode: playerModel.PlayerCode,
                    walletId: playerModel.DefaultWalletId,
                    currencyCode: playerModel.DefaultWalletCurrencyCode,
                    brandId: playerModel.BrandId,
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    status: RoundStatus.Open,
                    clientStatus: RoundClientStatus.Open,
                    externalCreatedOn: externalCreatedOn,
                    externalClosedOn: null,
                    bet: record.BetAmount.ToDecimal().GetValueOrDefault(0),
                    validBet: record.RealBetAmount.ToDecimal().GetValueOrDefault(0),
                    settle: 0,
                    commission: 0,
                    turnover: record.RealBetAmount.ToDecimal().GetValueOrDefault(0),
                    playerWinLoss: 0,
                    additionalData: JsonSerializer.Serialize(additionalData),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: game record exception.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
