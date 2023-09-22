using Tgs.GameInterface.AwcService.Protos;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record GamePullRecordCommand(string GameProviderCode, IEnumerable<GamePullRecord> Records);
