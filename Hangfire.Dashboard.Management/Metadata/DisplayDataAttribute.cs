using System;

namespace Hangfire.Dashboard.Management.Metadata
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class DisplayDataAttribute : Attribute
    {
        public string LabelText { get; set; }
        public string PlaceholderText { get; set; }

        public DisplayDataAttribute(string labelText, string placeholderText)
        {
            this.LabelText = labelText;
            this.PlaceholderText = placeholderText;
        }
    }
}