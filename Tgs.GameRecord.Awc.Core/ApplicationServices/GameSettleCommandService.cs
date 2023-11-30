using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class GameSettleCommandService(
    ILogger<GameSettleCommandService> logger,
    IUtensil utensil,
    IGameRecordHandleClient client) : ICommandService<GameSettleCommand>
{
    public async ValueTask ExecuteAsync(GameSettleCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                var isClosed = utensil.IsClosed(record.TxStatus);
                var isVoid = utensil.IsVoid(record.TxStatus, record.SettleStatus);
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
                    wagerState = utensil.GetWagerStates(isVoid, settleAmount);
                }

                await client
                    .MessageAsync(
                    gameProviderCode: command.GameProviderCode,
                    playerCode: record.UserId,
                    wagerState: wagerState,
                    wagerType: WagerTypes.Normal,
                    freeGameState: FreeGameStates.None,
                    gambleState: GambleStates.None,
                    cashoutState: CashoutStates.None,
                    resettleState: ResettleStates.None,
                    externalRoundId: record.RoundId,
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
                    additionalData: JsonNode.Parse(record.AdditionalData)!,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
