using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

internal class GameUnsettleCommandService : ICommandService<GameUnsettleCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly ILogger<GameUnsettleCommandService> _logger;

    public GameUnsettleCommandService(
        ILogger<GameUnsettleCommandService> logger,
        IGameRecordHandleClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(GameUnsettleCommand command, CancellationToken cancellationToken = default)
    {
        foreach (var record in command.Records)
        {
            try
            {
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
                    .MessageAsync(
                    gameProviderCode: command.GameProviderCode,
                    externalRoundId: record.ExternalRoundId,
                    externalGameId: record.GameCode,
                    playerCode: record.UserId,
                    status: GameRecordStatus.Open,
                    clientStatus: GameRecordClientStatus.Open,
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
                _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), record);
            }
        }
    }
}
