using StackExchange.Redis;

namespace Tgs.GameRecord.Awc.Core;

internal class ScheduleFormLock(
    IDatabase database) : IScheduleFormLock
{
    public Task<bool> LockingAsync(string gameProviderCode, long id)
        => database.StringSetAsync(key: $"awc:scheduleform:{gameProviderCode}:{id}", value: id, expiry: TimeSpan.FromMinutes(1), when: When.NotExists);

    public Task<bool> UnlockAsync(string gameProviderCode, long id)
        => database.KeyDeleteAsync(key: $"awc:scheduleform:{gameProviderCode}:{id}");
}
