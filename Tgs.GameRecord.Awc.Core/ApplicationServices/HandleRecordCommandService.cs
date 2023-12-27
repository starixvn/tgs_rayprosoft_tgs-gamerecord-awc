using System.Text.Json.Nodes;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class HandleRecordCommandService(
    IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var isClosed = command.Info.IsClosed;
        var isCanceled = command.Info.IsCanceled;
        var isVoid = command.Info.IsVoid;
        var isCashout = command.Info.IsCashout;
        var wagerState = WagerStates.Running;
        var cashoutState = CashoutStates.None;
        var externalCreatedOn = command.Info.ExternalCreatedOn.ToDateTimeOffset();
        var externalClosedOn = DateTimeOffset.UnixEpoch;
        var betAmount = command.Info.Bet.ToDecimal().GetValueOrDefault(0);
        var validBetAmount = command.Info.ValidBet.ToDecimal().GetValueOrDefault(0);
        var settleAmount = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;
        if (isClosed || isVoid)
        {
            externalClosedOn = command.Info.ExternalClosedOn.ToDateTimeOffset();
            settleAmount = command.Info.Settle.ToDecimal().GetValueOrDefault(0);
            turnover = command.Info.Turnover.ToDecimal().GetValueOrDefault(0);
            winLoss = command.Info.WinLoss.ToDecimal().GetValueOrDefault(0);
            wagerState = (isCashout) ? WagerStates.Refund : (isCanceled) ? WagerStates.Reject : (isVoid) ? WagerStates.Void : settleAmount > 0 ? WagerStates.Win : WagerStates.Lose;
            cashoutState = (isCashout) ? CashoutStates.Cashout : CashoutStates.None;
        }

        await client
            .MessageAsync(
            gameProviderCode: command.GameProviderCode,
            playerCode: command.Info.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: cashoutState,
            resettleState: ResettleStates.None,
            externalRoundId: command.Info.ExternalRoundId,
            externalWagerId: command.Info.ExternalWagerId,
            externalGameId: command.Info.ExternalGameId,
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
            additionalData: JsonNode.Parse(command.Info.AdditionalData)!,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
