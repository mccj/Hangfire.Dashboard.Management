using Hangfire.Annotations;
using System;

namespace Hangfire.Dashboard.Management.Pages
{
    partial class RecurringJobsPage
    {
    }
    internal class RecurringJobEntity
    {
        public static void ParseCronExpression([NotNull] string cronExpression)
        {
            var type = Type.GetType("Hangfire.RecurringJobEntity,Hangfire.Core");
            var render = type.GetMethod("ParseCronExpression");
            var cc = render.Invoke(null, new[] { cronExpression });
        }
    }

}