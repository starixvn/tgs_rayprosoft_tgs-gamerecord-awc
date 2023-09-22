using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record GameSettleCommand(string GameProviderCode, IEnumerable<GameSettleRecord> Records);
