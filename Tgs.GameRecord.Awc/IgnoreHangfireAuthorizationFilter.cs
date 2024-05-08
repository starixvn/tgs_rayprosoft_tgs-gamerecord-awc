using Hangfire.Dashboard;

namespace Tgs.GameRecord.Awc;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
