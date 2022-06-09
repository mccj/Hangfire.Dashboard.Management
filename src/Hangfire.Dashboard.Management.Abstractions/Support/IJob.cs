namespace Hangfire.Dashboard.Management.Support
{
    //public interface IJob
    //{
    //}
    /// <summary>
    /// 可管理任务定义
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class JobAttribute : System.Attribute
    { }
}