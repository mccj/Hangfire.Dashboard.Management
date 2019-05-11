using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Hangfire.Dashboard.Management.Service
{
    public class DashboardAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
    {//Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter 
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}