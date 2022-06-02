using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    public record GaemUnsettleCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameUnsettleTransaction> Transactions);
}
