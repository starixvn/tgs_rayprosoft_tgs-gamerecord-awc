using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

internal class GameUnsettleCommandService(
    ILogger<GameUnsettleCommandService> logger,
    IGameRecordHandleClient client) : ICommandService<GameUnsettleCommand>
{
    public async ValueTask ExecuteAsync(GameUnsettleCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
                var externalCreatedOn = DateTimeOffset.ParseExact(record.BetTime, @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                var betAmount = record.BetAmount.ToDecimal().GetValueOrDefault(0);
                var validBetAmount = record.RealBetAmount.ToDecimal().GetValueOrDefault(0);

                await client.MessageAsync(
                    gameProviderCode: command.GameProviderCode,
                    playerCode: record.UserId,
                    wagerState: WagerStates.Running,
                    wagerType: WagerTypes.Normal,
                    freeGameState: FreeGameStates.None,
                    gambleState: GambleStates.None,
                    cashoutState: CashoutStates.None,
                    resettleState: ResettleStates.None,
                    externalRoundId: record.RoundId,
                    externalWagerId: record.ExternalRoundId,
                    externalGameId: record.GameCode,
                    externalCreatedOn: externalCreatedOn,
                    externalClosedOn: DateTimeOffset.UnixEpoch,
                    externalPayoffOn: DateTimeOffset.UnixEpoch,
                    externalResettleOn: DateTimeOffset.UnixEpoch,
                    bet: betAmount,
                    validBet: validBetAmount,
                    settle: 0.0m,
                    commission: 0.0m,
                    turnover: 0.0m,
                    winLoss: 0.0m,
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
