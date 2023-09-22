namespace Tgs.GameRecord.Awc.Core;

public interface IScheduleFormLock
{
    public Task<bool> LockingAsync(string gameProviderCode, long id);

    public Task<bool> UnlockAsync(string gameProviderCode, long id);
}
