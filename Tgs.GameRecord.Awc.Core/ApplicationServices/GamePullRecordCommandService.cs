using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class GamePullRecordCommandService : ICommandService<GamePullRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly ILogger<GamePullRecordCommandService> _logger;
    private readonly IUtensil _utensil;

    public GamePullRecordCommandService(
        ILogger<GamePullRecordCommandService> logger,
        IUtensil utensil,
        IGameRecordHandleClient client)
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
                var isClosed = _utensil.IsClosed(record.TxStatus);
                var isVoid = _utensil.IsVoid(record.TxStatus, record.SettleStatus);
                var wagerState = WagerStates.Running;
                var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                var externalClosedOn = DateTimeOffset.UnixEpoch;
                var betAmount = record.BetAmount.ToDecimal().GetValueOrDefault(0);
                var validBetAmount = record.RealBetAmount.ToDecimal().GetValueOrDefault(0);
                var settleAmount = 0.0m;
                var turnover = 0.0m;
                var winLoss = 0.0m;
                if (isClosed || isVoid)
                {
                    externalClosedOn = DateTimeOffset.ParseExact(record.TxTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    settleAmount = record.RealWinAmount.ToDecimal().GetValueOrDefault(0);
                    turnover = record.Turnover.ToDecimal().GetValueOrDefault(0);
                    winLoss = settleAmount - validBetAmount;
                    wagerState = _utensil.GetWagerStates(isVoid, settleAmount);
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
                    .MessageAsync(
                    gameProviderCode: command.GameProviderCode,
                    playerCode: record.UserId,
                    wagerState: wagerState,
                    wagerType: WagerTypes.Normal,
                    freeGameState: FreeGameStates.None,
                    gambleState: GambleStates.None,
                    cashoutState: CashoutStates.None,
                    resettleState: ResettleStates.None,
                    externalRoundId: record.ExternalRoundId,
                    externalWagerId: record.ExternalRoundId,
                    externalGameId: record.GameCode,
                    externalCreatedOn: externalCreatedOn,
                    externalClosedOn: externalClosedOn,
                    externalPayoffOn: externalClosedOn,
                    externalResettleOn: DateTimeOffset.UnixEpoch,
                    bet: betAmount,
                    validBet: validBetAmount,
                    settle: settleAmount,
                    commission: 0.0m,
                    turnover: turnover,
                    winLoss: winLoss,
                    additionalData: JsonSerializer.Serialize(additionalData),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
