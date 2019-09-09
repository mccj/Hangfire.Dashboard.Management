using Hangfire.Annotations;
using System;

namespace Hangfire.Dashboard.Management.Pages
{
    partial class JobDetailsPage
    {
        public JobDetailsPage(string jobId)
        {
            JobId = jobId;
        }

        public string JobId { get; }
    }
    internal struct Continuation
    {
        public string JobId { get; set; }
        public JobContinuationOptions Options { get; set; }
    }
    internal static class JobMethodCallRenderer
    {
        public static NonEscapedString Render(Common.Job job)
        {
            if (job == null) { return new NonEscapedString("<em>"+ Hangfire.Dashboard.Resources.Strings.Common_CannotFindTargetMethod + "</em>"); }

            var type = Type.GetType("Hangfire.Dashboard.JobMethodCallRenderer,Hangfire.Core");
            var render = type.GetMethod("Render");
            return render.Invoke(null, new[] { job }) as NonEscapedString;
        }
    }
}
