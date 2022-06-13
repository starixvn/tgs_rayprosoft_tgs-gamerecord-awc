using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    public record GamePullRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceAwcServiceGamePullRecord> Records);
}
