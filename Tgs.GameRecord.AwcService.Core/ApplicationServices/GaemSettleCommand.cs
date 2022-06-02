using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    public record GaemSettleCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameSettleTransaction> Transactions);
}
