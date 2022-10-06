using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameRecord.AwcService.Core
{
    internal class UtensilRepository : IUtensil
    {
        private readonly IGameProvidersQueryServerClient _client;

        public UtensilRepository(
            IGameProvidersQueryServerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
            => _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId);

        public async ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
        {
            var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);
            return gameModel?.GameId;
        }

        public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        {
            var gameProviderId = await _client.GameProviderGetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
            return gameProviderId!.Value;
        }

        public GameRoundClientStatus GetGameRoundClientStatus(GameRoundStatus gameRoundStatus, decimal playerWinLose)
            => gameRoundStatus switch
            {
                GameRoundStatus.Void => GameRoundClientStatus.Void,
                GameRoundStatus.Closed => (playerWinLose > 0) ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss,
                _ => GameRoundClientStatus.Open
            };

        public GameRoundStatus GetGameRoundStatus(int txStatus, int settleStatus)
            => txStatus switch
            {
                0 => GameRoundStatus.Open,
                2 => GameRoundStatus.Void,
                1 => (settleStatus is 0 or 1) ? GameRoundStatus.Closed : GameRoundStatus.Void,
                _ => GameRoundStatus.Void
            };

        public ValueTask<PlayerModel?> GetPlayerAsync(string playerCode)
            => _client.PlayerGetAsync(playerCode);
    }
}
