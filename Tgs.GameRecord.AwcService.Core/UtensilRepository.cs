using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.AwcService.Core;

internal class UtensilRepository : IUtensil
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly IGameCommandClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IMemoryCache _memoryCache;

    public UtensilRepository(
        IOptions<GameRecordInfoOptions> options,
        IGameCommandClient client,
        IMemoryCache memoryCache,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:{_gameRecordInfo.GameProviderCode}:{brandId}:{gameProviderId}";

        if (!_memoryCache.TryGetValue(key, out Guid gameProviderConfigurationId))
        {
            gameProviderConfigurationId = await _client
                .BrandGetGameProviderConfigurationIdAsync(
                    brandId: brandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"The game provider configuration ID does not exist.({brandId}|{gameProviderId})");

            return _memoryCache
                .Set(
                key: key,
                value: gameProviderConfigurationId,
                absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }
        return gameProviderConfigurationId;
    }

    public GameRecordClientStatus GetClientStatus(
        bool isClosed,
        bool isVoid,
        decimal playerWinLose)
        => !isClosed ? GameRecordClientStatus.Open : isVoid ? GameRecordClientStatus.Void : playerWinLose > 0 ? GameRecordClientStatus.Win : GameRecordClientStatus.Loss;

    public async ValueTask<int> GetGameIdAsync(
            Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:{_gameRecordInfo.GameProviderCode}:{gameProviderId}:{gameExternalId}";
        if (!_memoryCache.TryGetValue(key, out int gameId))
        {
            gameId = await _client
                .GameGetIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: gameExternalId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This game does not exist.({gameExternalId})");

            return _memoryCache
                .Set(
                key: key,
                value: gameId);
        }
        return gameId;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:{_gameRecordInfo.GameProviderCode}:{gameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out Guid gameProviderId))
        {
            gameProviderId = await _client
                .GameProviderGetGameProviderIdAsync(
                    gameProviderCode: gameProviderCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"The identifier for this game provider does not exist.({gameProviderCode})");

            return _memoryCache.Set(key, gameProviderId);
        }
        return gameProviderId;
    }

    public async ValueTask<PlayerInfo> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:{_gameRecordInfo.GameProviderCode}:{playerCode}";
        if (!_memoryCache.TryGetValue(key, out PlayerInfo? playerModel))
        {
            playerModel = await _client
                .PlayerGetAsync(
                playerCode: playerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"This player does not exist.({playerCode})");

            return _memoryCache
                .Set(
                    key: key,
                    value: playerModel,
                    absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }
        return playerModel ?? throw new NullReferenceException($"This player does not exist.({playerCode})");
    }

    public bool IsClosed(
        int txStatus)
        => txStatus switch
        {
            0 => false,
            _ => true
        };

    public bool IsVoid(
        int txStatus,
        int settleStatus)
        => txStatus switch
        {
            0 => false,
            2 => true,
            1 => settleStatus is not (0 or 1),
            _ => true
        };
}
