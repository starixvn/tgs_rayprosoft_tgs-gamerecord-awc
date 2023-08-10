using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly IGameRoundsWorkClient _client;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IGameRoundsWorkClient client,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordCommand command, CancellationToken cancellationToken = default)
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

                var isClosed = _utensil.IsClosed(record.TxStatus);
                var isVoid = _utensil.IsVoid(record.TxStatus, record.SettleStatus);
                var status = isClosed ? RoundStatus.Closed : RoundStatus.Open;
                var clientStatus = _utensil.GetGameRoundClientStatus(isClosed, isVoid, record.RealWinAmount);
                var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);

                DateTimeOffset? externalClosedOn = null;
                var betAmount = record.BetAmount.ToDecimal().GetValueOrDefault(0);
                var validBetAmount = record.RealBetAmount.ToDecimal().GetValueOrDefault(0);
                var settleAmount = 0.0m;
                var turnover = 0.0m;
                var playerWinLoss = 0.0m;
                if (status == RoundStatus.Closed)
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
                    status: status,
                    clientStatus: clientStatus,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: game record exception.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
