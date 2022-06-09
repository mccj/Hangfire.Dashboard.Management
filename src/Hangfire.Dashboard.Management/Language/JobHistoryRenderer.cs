using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Resources;

namespace Hangfire.Dashboard.Management
{
    public class JobHistoryRenderer
    {
        public static void Register(System.Resources.ResourceManager resource)
        {
            var jobHistoryRenderer = new JobHistoryRendererInfo(resource);
            jobHistoryRenderer.Register(SucceededState.StateName, jobHistoryRenderer.SucceededRenderer);
            //Hangfire.Dashboard.JobHistoryRenderer.Register(FailedState.StateName, jobHistoryRenderer.FailedRenderer);
            jobHistoryRenderer.Register(ProcessingState.StateName, jobHistoryRenderer.ProcessingRenderer);
            jobHistoryRenderer.Register(EnqueuedState.StateName, jobHistoryRenderer.EnqueuedRenderer);
            jobHistoryRenderer.Register(ScheduledState.StateName, jobHistoryRenderer.ScheduledRenderer);
            //Hangfire.Dashboard.JobHistoryRenderer.Register(DeletedState.StateName, jobHistoryRenderer.NullRenderer);
            jobHistoryRenderer.Register(AwaitingState.StateName, jobHistoryRenderer.AwaitingRenderer);
        }
    }

    public class JobHistoryRendererInfo
    {
        private ResourceManager resource;

        private static readonly IDictionary<string, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString>>
           _renderers = new Dictionary<string, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString>>();

        public JobHistoryRendererInfo(ResourceManager resource)
        {
            this.resource = resource;
        }

        public void Register(string state, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString> renderer)
        {
            var type = Type.GetType("Hangfire.Dashboard.JobHistoryRenderer,Hangfire.Core");
            var render = type.GetField("Renderers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
            var renderers = render.GetValue(null) as IDictionary<string, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString>>;

            if (renderers.ContainsKey(state))
            {
                var _state = renderer.Method.Name;
                var ddd = renderers[state];
                _register(_state, ddd);
            }
            Hangfire.Dashboard.JobHistoryRenderer.Register(state, renderer);
        }

        private void _register(string state, Func<HtmlHelper, IDictionary<string, string>, NonEscapedString> renderer)
        {
            if (!_renderers.ContainsKey(state))
            {
                _renderers.Add(state, renderer);
            }
            else
            {
                _renderers[state] = renderer;
            }
        }

        //private bool _exists(string state)
        //{
        //    return _renderers.ContainsKey(state);
        //}
        private NonEscapedString _renderHistory(
             HtmlHelper helper,
            string state, IDictionary<string, string> properties)
        {
            var renderer = _renderers.ContainsKey(state)
                ? _renderers[state]
                : Hangfire.Dashboard.JobHistoryRenderer.DefaultRenderer;

            return renderer?.Invoke(helper, properties);
        }

        public NonEscapedString ScheduledRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var rendered = _renderHistory(helper, System.Reflection.MethodBase.GetCurrentMethod().Name, stateData)?.ToString();
            var builder = rendered?
                     .Replace("<dt>Enqueue at:</dt>", $"<dt>{resource.GetString("Enqueue at:")}</dt>")
                     ;
            return new NonEscapedString(builder.ToString());
        }

        public NonEscapedString SucceededRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var rendered = _renderHistory(helper, System.Reflection.MethodBase.GetCurrentMethod().Name, stateData).ToString();
            rendered = System.Text.RegularExpressions.Regex.Replace(rendered, "<dd>(\\d+\\.?\\d*)(?<name>\\w+)</dd>", m =>
            {
                var name = m.Groups["name"].Value;
                return m.Value.Replace(name, resource.GetString(name));
            });
            var builder = rendered?
                     .Replace("<dt>Latency:</dt>", $"<dt>{resource.GetString("Latency:")}</dt>")
                     .Replace("<dt>Duration:</dt>", $"<dt>{resource.GetString("Duration:")}</dt>")
                     .Replace("<dt>Result:</dt>", $"<dt>{resource.GetString("Result:")}</dt>")
                     ;
            return new NonEscapedString(builder.ToString());
        }

        public NonEscapedString ProcessingRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var rendered = _renderHistory(helper, System.Reflection.MethodBase.GetCurrentMethod().Name, stateData)?.ToString();
            rendered = System.Text.RegularExpressions.Regex.Replace(rendered, "<span data-moment-title=\"(?<timestamp>\\d+)\">(?<time>\\+(\\d+\\.?\\d*))(?<name>\\w+)</span>", m =>
            {
                var timestamp = m.Groups["timestamp"].Value;
                var time = m.Groups["time"].Value;
                var name = m.Groups["name"].Value;
                return $"<span data-moment-title=\"{timestamp}\">{time}{resource.GetString(name)}</span>";
            });
            var builder = rendered?
                     .Replace("<dt>Server:</dt>", $"<dt>{resource.GetString("Server:")}</dt>")
                     .Replace("<dt>Worker:</dt>", $"<dt>{resource.GetString("Worker:")}</dt>")
                     ;
            return new NonEscapedString(builder.ToString());
        }

        public NonEscapedString EnqueuedRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var rendered = _renderHistory(helper, System.Reflection.MethodBase.GetCurrentMethod().Name, stateData)?.ToString();
            var builder = rendered?
                     .Replace("<dt>Queue:</dt>", $"<dt>{resource.GetString("Queue:")}</dt>")
                     .Replace("<dt>Worker:</dt>", $"<dt>{resource.GetString("Worker:")}</dt>")
                     ;
            return new NonEscapedString(builder.ToString());
        }

        public NonEscapedString AwaitingRenderer(HtmlHelper helper, IDictionary<string, string> stateData)
        {
            var rendered = _renderHistory(helper, System.Reflection.MethodBase.GetCurrentMethod().Name, stateData)?.ToString();
            var builder = rendered?
                     .Replace("<dt>Parent:</dt>", $"<dt>{resource.GetString("Parent:")}</dt>")
                     .Replace("<dt>Next State:</dt>", $"<dt>{resource.GetString("Next State:")}</dt>")
                     .Replace("<dt>Options:</dt>", $"<dt>{resource.GetString("Options:")}</dt>")
                    ;
            return new NonEscapedString(builder.ToString());
        }
    }
}