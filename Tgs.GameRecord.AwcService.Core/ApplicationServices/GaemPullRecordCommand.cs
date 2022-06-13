using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices
{
    public record GaemPullRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceAwcServiceGamePullRecord> Records);
}
