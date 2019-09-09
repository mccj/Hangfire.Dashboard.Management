using Hangfire.Common;
using Hangfire.States;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace Hangfire.Dashboard.Management
{
    public class JobHistoryRenderer
    {
        public static void Register(System.Resources.ResourceManager resource)
        {
            var jobHistoryRenderer = new JobHistoryRendererInfo(resource);
            Hangfire.Dashboard.JobHistoryRenderer.Register(SucceededState.StateName, jobHistoryRenderer.SucceededRenderer);
            //Hangfire.Dashboard.JobHistoryRenderer.Register(FailedState.StateName, jobHistoryRenderer.FailedRenderer);
            Hangfire.Dashboard.JobHistoryRenderer.Register(ProcessingState.StateName, jobHistoryRenderer.ProcessingRenderer);
            Hangfire.Dashboard.JobHistoryRenderer.Register(EnqueuedState.StateName, jobHistoryRenderer.EnqueuedRenderer);
            Hangfire.Dashboard.JobHistoryRenderer.Register(ScheduledState.StateName, jobHistoryRenderer.ScheduledRenderer);
            //Hangfire.Dashboard.JobHistoryRenderer.Register(DeletedState.StateName, jobHistoryRenderer.NullRenderer);
            Hangfire.Dashboard.JobHistoryRenderer.Register(AwaitingState.StateName, jobHistoryRenderer.AwaitingRenderer);
        }
    }

    public class JobHistoryRendererInfo
    {
        private ResourceManager resource;

        public JobHistoryRendererInfo(ResourceManager resource)
        {
            this.resource = resource;
        }

        public NonEscapedString ScheduledRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var enqueueAt = JobHelper.DeserializeDateTime(stateData["EnqueueAt"]);

            return new NonEscapedString(
                $"<dl class=\"dl-horizontal\"><dt>{resource.GetString("Enqueue at:")}</dt><dd data-moment=\"{JobHelper.ToTimestamp(enqueueAt)}\">{enqueueAt}</dd></dl>");
        }
        public NonEscapedString SucceededRenderer(HtmlHelper html, IDictionary<string, string> stateData)
        {
            var builder = new StringBuilder();
            builder.Append("<dl class=\"dl-horizontal\">");

            var itemsAdded = false;

            if (stateData.ContainsKey("Latency"))
            {
                var latency = TimeSpan.FromMilliseconds(long.Parse(stateData["Latency"]));

                builder.Append($"<dt>{resource.GetString("Latency:")}</dt><dd>{html.ToHumanDurationTr(resource, latency, false)}</dd>");

                itemsAdded = true;
            }

            if (stateData.ContainsKey("PerformanceDuration"))
            {
                var duration = TimeSpan.FromMilliseconds(long.Parse(stateData["PerformanceDuration"]));
                builder.Append($"<dt>{resource.GetString("Duration:")}</dt><dd>{html.ToHumanDurationTr(resource, duration, false)}</dd>");

                itemsAdded = true;
            }


            if (stateData.ContainsKey("Result") && !String.IsNullOrWhiteSpace(stateData["Result"]))
            {
                var result = stateData["Result"];
                builder.Append($"<dt>{resource.GetString("Result:")}</dt><dd>{System.Net.WebUtility.HtmlEncode(result)}</dd>");

                itemsAdded = true;
            }

            builder.Append("</dl>");

            if (!itemsAdded) return null;

            return new NonEscapedString(builder.ToString());
        }
        public NonEscapedString ProcessingRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var builder = new StringBuilder();
            builder.Append("<dl class=\"dl-horizontal\">");

            string serverId = null;

            if (stateData.ContainsKey("ServerId"))
            {
                serverId = stateData["ServerId"];
            }
            else if (stateData.ContainsKey("ServerName"))
            {
                serverId = stateData["ServerName"];
            }

            if (serverId != null)
            {
                builder.Append($"<dt>{resource.GetString("Server:")}</dt>");
                builder.Append($"<dd>{helper.ServerId(serverId)}</dd>");
            }

            if (stateData.ContainsKey("WorkerId"))
            {
                builder.Append($"<dt>{resource.GetString("Worker:")}</dt>");
                builder.Append($"<dd>{stateData["WorkerId"].Substring(0, 8)}</dd>");
            }
            else if (stateData.ContainsKey("WorkerNumber"))
            {
                builder.Append($"<dt>{resource.GetString("Worker:")}</dt>");
                builder.Append($"<dd>#{stateData["WorkerNumber"]}</dd>");
            }

            builder.Append("</dl>");

            return new NonEscapedString(builder.ToString());
        }

        public NonEscapedString EnqueuedRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            return new NonEscapedString(
                $"<dl class=\"dl-horizontal\"><dt>{resource.GetString("Queue:")}</dt><dd>{helper.QueueLabel(stateData["Queue"])}</dd></dl>");
        }
        public NonEscapedString AwaitingRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var builder = new StringBuilder();

            builder.Append("<dl class=\"dl-horizontal\">");

            if (stateData.ContainsKey("ParentId"))
            {
                builder.Append($"<dt>{resource.GetString("Parent")}</dt><dd>{helper.JobIdLink(stateData["ParentId"])}</dd>");
            }

            if (stateData.ContainsKey("NextState"))
            {
                var nextState = JsonConvert.DeserializeObject<IState>(
                    stateData["NextState"],
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                builder.Append($"<dt>{resource.GetString("Next State")}</dt><dd>{helper.StateLabel(nextState.Name)}</dd>");
            }

            if (stateData.ContainsKey("Options"))
            {
                builder.Append($"<dt>{resource.GetString("Options")}</dt><dd><code>{helper.HtmlEncode(stateData["Options"])}</code></dd>");
            }

            builder.Append("</dl>");

            return new NonEscapedString(builder.ToString());
        }

    }
}
