using Tgs.GameRecord.Awc.Core.ApplicationServices;

namespace Tgs.GameRecord.Awc.Core;

public interface IManualPullRecordDispatcher
{
	ValueTask PushCommandAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default);
}
