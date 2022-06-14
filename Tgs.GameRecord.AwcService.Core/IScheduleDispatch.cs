using Tgs.GameRecord.AwcService.Core.DispatchServices;

namespace Tgs.GameRecord.AwcService.Core
{
    public interface IScheduleDispatch
    {
        public Task<bool> ExistAsync(string key);

        public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command);

        public Task<bool> UnlockAsync(string key);
    }
}
