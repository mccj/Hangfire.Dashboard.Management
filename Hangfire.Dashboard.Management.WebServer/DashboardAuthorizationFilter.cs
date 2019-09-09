using Hangfire.Annotations;

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