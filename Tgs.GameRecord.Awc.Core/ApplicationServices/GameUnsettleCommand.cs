using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record GameUnsettleCommand(string GameProviderCode, IEnumerable<GameUnsettleRecord> Records);
