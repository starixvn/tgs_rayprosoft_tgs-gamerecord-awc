using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.AwcService.Core.ApplicationServices;

public record GameUnsettleCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameUnsettleRecord> Records);
