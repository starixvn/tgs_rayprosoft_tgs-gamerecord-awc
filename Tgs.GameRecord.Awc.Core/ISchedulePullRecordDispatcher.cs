using Tgs.GameRecord.Awc.Core.ApplicationServices;

namespace Tgs.GameRecord.Awc.Core;

public interface ISchedulePullRecordDispatcher
{
	ValueTask PushCommandAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default);
}
