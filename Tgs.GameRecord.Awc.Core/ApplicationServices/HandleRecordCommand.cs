using Tgs.GameInterface.Awc.Protos;

namespace Tgs.GameRecord.Awc.Core.ApplicationServices;

public record HandleRecordCommand(
    string GameProviderCode,
    GameWagerInfo Info);
