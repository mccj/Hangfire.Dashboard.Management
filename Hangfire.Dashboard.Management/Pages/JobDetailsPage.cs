using Hangfire.Annotations;
using System;
using System.Collections.Generic;

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
            //if (job == null) { return new NonEscapedString("<em>" + Hangfire.Dashboard.Resources.Strings.Common_CannotFindTargetMethod + "</em>"); }

            var type = Type.GetType("Hangfire.Dashboard.JobMethodCallRenderer,Hangfire.Core");
            var render = type.GetMethod("Render");
            return render.Invoke(null, new[] { job }) as NonEscapedString;
        }
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
    internal static class ContinuationsSupportAttributeEx
    {
        internal static List<Continuation> DeserializeContinuations(string serialized)
        {
            var type = Type.GetType("Hangfire.ContinuationsSupportAttribute,Hangfire.Core");
            var render = type.GetMethod("DeserializeContinuations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            return render.Invoke(null, new[] { serialized }) as List<Continuation>;

        }
    }
}
