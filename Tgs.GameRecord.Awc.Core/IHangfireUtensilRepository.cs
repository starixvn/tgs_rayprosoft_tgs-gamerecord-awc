using Tgs.GameRecord.Awc.Core.JobApplicationServices;

namespace Tgs.GameRecord.Awc.Core;

public interface IHangfireUtensilRepository
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
