using System;
using System.Linq;
using System.Reflection;

namespace Hangfire.Dashboard.Management.Metadata
{
    public class JobMetadata
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool? HideJobSnippetCode { get; set; }
        public bool? DisabledQueueSetting { get; set; }

        public string Queue { get; set; }
        public Type Type { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public JobParameter[] Parameters { get; set; }

        public string GetId()
        {
            if (string.IsNullOrWhiteSpace(_md5) && this.MethodInfo != null)
            {
                _md5 = (this.MethodInfo.ReflectedType.AssemblyQualifiedName + this.MethodInfo.Name + string.Join(",", this.MethodInfo.GetParameters().Select(f => f.Name))).GetMd5();
            }
            return _md5;
        }

        private string _md5 = string.Empty;
    }

    public class JobParameter
    {
        public Type ParameterType { get; set; }
        public string Name { get; set; }
        public string LabelText { get; set; }
        public string PlaceholderText { get; set; }
        public string DescriptionText { get; set; }
        public bool? IsMultiLine { get; set; }
        public Type ConvertType { get; set; }
        public object DefaultValue { get; set; }
        public bool? Readonly { get; set; }
        public string CssClasses { get; set; }
        public string InputMask { get; set; }
        public bool? Required { get; set; }
    }
}