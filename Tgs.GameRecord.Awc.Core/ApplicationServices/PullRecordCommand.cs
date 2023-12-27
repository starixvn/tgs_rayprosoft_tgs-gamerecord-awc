using Tgs.GameInterface.Awc.Protos;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record PullRecordCommand(string GameProviderCode, IEnumerable<GameWagerInfo> Infos);
