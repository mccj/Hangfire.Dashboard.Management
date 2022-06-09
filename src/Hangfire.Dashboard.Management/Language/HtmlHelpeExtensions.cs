using System.Text;

namespace System.Linq
{
    internal static class HtmlHelpeExtensions
    {
        public static string ToHumanDurationTr(this Hangfire.Dashboard.HtmlHelper htmlHelper, Resources.ResourceManager resource, TimeSpan? duration, bool displaySign = true)
        {
            if (duration == null) return null;

            var builder = new StringBuilder();
            if (displaySign)
            {
                builder.Append(duration.Value.TotalMilliseconds < 0 ? "-" : "+");
            }

            duration = duration.Value.Duration();

            if (duration.Value.Days > 0)
            {
                builder.Append($"{duration.Value.Days}{resource.GetString("d")} ");
            }

            if (duration.Value.Hours > 0)
            {
                builder.Append($"{duration.Value.Hours}{resource.GetString("h")} ");
            }

            if (duration.Value.Minutes > 0)
            {
                builder.Append($"{duration.Value.Minutes}{resource.GetString("m")} ");
            }

            if (duration.Value.TotalHours < 1)
            {
                if (duration.Value.Seconds > 0)
                {
                    builder.Append(duration.Value.Seconds);
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($".{duration.Value.Milliseconds.ToString().PadLeft(3, '0')}");
                    }

                    builder.Append($"{resource.GetString("s")} ");
                }
                else
                {
                    if (duration.Value.Milliseconds > 0)
                    {
                        builder.Append($"{duration.Value.Milliseconds}{resource.GetString("ms")} ");
                    }
                }
            }

            if (builder.Length <= 1)
            {
                builder.Append($" <1{resource.GetString("ms")} ");
            }

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }
}