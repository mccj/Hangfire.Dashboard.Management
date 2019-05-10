using System;

namespace Hangfire.Dashboard.Management.Metadata
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class DisplayDataAttribute : Attribute
    {
        public string LabelText { get; set; }
        public string PlaceholderText { get; set; }
        public string DescriptionText { get; set; }
        public string DefaultValue { get; set; }
        public bool IsMultiLine { get; set; }
        public Type ConvertType { get; set; }
        public DisplayDataAttribute(string labelText = null, string placeholderText = null, string descriptionText = null, string defaultValue = null)
        {
            this.LabelText = labelText;
            this.PlaceholderText = placeholderText;
            this.DescriptionText = descriptionText;
            this.DefaultValue = defaultValue;
        }
    }

    public interface IInputDataList
    {
        System.Collections.Generic.Dictionary<string, string> GetData();
        string GetDefaultValue();
    }
}