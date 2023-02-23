using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.ManualWork.Client;
using Tgs.GameRounds.ManualWork.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GameUnsettleCommandService : ICommandService<GameUnsettleCommand>
{
    private readonly IGameRoundsManualWorkClient _client;
    private readonly ILogger<GameUnsettleCommandService> _logger;
    private readonly IUtensil _utensil;

    public GameUnsettleCommandService(
        ILogger<GameUnsettleCommandService> logger,
        IGameRoundsManualWorkClient client,
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

                if (success)
                {
                    await _client
                    .RoundBidding(
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
                        status: RoundBiddingStatus.Open,
                        clientStatus: RoundBiddingClientStatus.Open,
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
                else
                {
                    await _client
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
                _logger.LogError(ex, "遊戲未結算處理發生例外.({record})", record);
            }
        }
    }
}
