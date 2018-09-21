using Hangfire.Annotations;
using System;

namespace Hangfire.Dashboard.Management.Standard.Test
{
    public class LocalRequestsOnlyAuthorizationFilter : IDashboardAuthorizationFilter
    //#if NETFULL
    //#pragma warning disable 618
    //        , IAuthorizationFilter
    //#pragma warning restore 618
    //#endif
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
            // if unknown, assume not local
            if (String.IsNullOrEmpty(context.Request.RemoteIpAddress))
                return false;

            // check if localhost
            if (context.Request.RemoteIpAddress == "127.0.0.1" || context.Request.RemoteIpAddress == "::1")
                return true;

            // compare with local address
            if (context.Request.RemoteIpAddress == context.Request.LocalIpAddress)
                return true;

            return false;
        }

        //#if NETFULL
        //        public bool Authorize(IDictionary<string, object> owinEnvironment)
        //        {
        //            var context = new Microsoft.Owin.OwinContext(owinEnvironment);

        //            // if unknown, assume not local
        //            if (String.IsNullOrEmpty(context.Request.RemoteIpAddress))
        //                return false;

        //            // check if localhost
        //            if (context.Request.RemoteIpAddress == "127.0.0.1" || context.Request.RemoteIpAddress == "::1")
        //                return true;

        //            // compare with local address
        //            if (context.Request.RemoteIpAddress == context.Request.LocalIpAddress)
        //                return true;

        //            return false;
        //        }
        //#endif
    }

}
