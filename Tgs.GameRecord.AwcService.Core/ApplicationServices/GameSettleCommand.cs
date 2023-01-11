using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

public record GameSettleCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceAwcServiceGameSettleRecord> Records);
