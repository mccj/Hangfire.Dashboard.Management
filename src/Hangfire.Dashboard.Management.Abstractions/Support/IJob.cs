namespace Hangfire.Dashboard.Management.Support
{
    /// <summary>
    /// 可管理任务定义
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class JobAttribute : System.Attribute
    {
        public bool HideJobSnippetCode { get; set; }
        public bool DisabledQueueSetting { get; set; }
        public string Queue { get; set; }
    }
}