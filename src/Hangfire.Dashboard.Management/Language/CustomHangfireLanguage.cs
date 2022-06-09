using System.Globalization;
using System.Resources;

namespace Hangfire.Dashboard.Management
{
    public class CustomHangfireLanguage : System.Resources.ResourceManager
    {
        private ResourceManager _resourceManager;
        private CultureInfo _culture;
        private System.Func<string, CultureInfo, string> _translateFunc;

        public CustomHangfireLanguage(ResourceManager resourceManager, System.Func<string, CultureInfo, string> translateFunc, CultureInfo culture)
        {
            this._resourceManager = resourceManager;
            this._translateFunc = translateFunc;
            this._culture = culture;
        }

        public override string GetString(string name, CultureInfo culture)
        {
            culture = culture ?? this._culture;
            if (this._translateFunc != null)
            {
                var r = this._translateFunc(name, culture);
                if (null != r) return r;
            }

            //if (name == "JobsSidebarMenu_Enqueued")
            //{
            //    return "队列";
            //}
            //else if (name == "JobDetailsPage_JobId")
            //{
            //    return "任务编号";
            //}
            //else if (name == "SucceededJobsPage_Table_TotalDuration")
            //{
            //    return "消耗时间";
            //}
            //else if (name == "LayoutPage_Footer_Generatedms")
            //{
            //    return "耗时: {0} 毫秒";
            //}
            //else if (name == "JobDetailsPage_JobFinished_Warning_Html")
            //{
            //    return "<strong>任务完成</strong>.系统将在<em><abbr data-moment=\"{0}\">{1}</abbr></em>被自动删除这个任务记录.";
            //}
            //else if (name == "FetchedJobsPage_Title")
            //{
            //    return "任务列队";
            //}
            //else if (name == "Common_CannotFindTargetMethod")
            //{
            //    return "找不到目标方法。";
            //}
            //else if (name == "FailedJobsPage_FailedJobsNotExpire_Warning_Html")
            //{
            //    return "<strong>失败的作业不会过期。</strong> 允许您在没有任何时间压力的情况下重新排队。你应该重新排队或手动删除它们， 或者添加 <code>AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)</code> 属性，系统将会自动删除作业。";
            //}
            //else if (name == "JobDetailsPage_JobAbortedNotActive_Warning_Html")
            //{
            //    return "<strong>工作被中止</strong> – 它现在正在由非<a href=\"{1}\">活动状态的服务器</a> <code>{0}</code>处理任务。 系统将在任务重试超时后自动结束，你也可以手动重新执行或将其删除。";
            //}
            //else if (name == "RetriesPage_Warning_Html")
            //{
            //    return "<h4>重试的工作，但是这个页面无法显示</h4><p>别担心，重试执行工作是正常的情况。您当前的作业存储不支持显示此页所需的一些查询。请更新您的存储或完善相关指令集。</p><p>请前往< a href =\"{0}\">计划作业</a>页面查看所有预定的工作及重试任务。</p>";
            //}
            //else if (name == "Metrics_FailedCountOrNull")
            //{
            //    return "{0} 没有发现工作任务。请手动重试或删除它们。";
            //}
            //else if (name == "AwaitingJobsPage_ContinuationsWarning_Text")
            //{
            //    return "别担心，系统正在按照预定指令执行当前的工作任务。但是，当前的作业存储不支持显示此页所需的一些查询。请尝试更新您的存储，或者等到完整命令集实现为止。";
            //}
            //else if (name == "Metrics_ActiveConnections")
            //{
            //    return "活动连接";
            //}
            //else if (name == "Metrics_RecurringJobs")
            //{
            //    return "定时任务";
            //}
            //else if (name == "StateName_Created")
            //{
            //    return "创建";
            //}
            //else if (name == "StateName_Enqueued")
            //{
            //    return "排队";
            //}
            //else if (name == "StateName_Processing")
            //{
            //    return "处理";
            //}
            //else if (name == "StateName_Succeeded")
            //{
            //    return "完成";
            //}
            //else if (name == "StateName_Failed")
            //{
            //    return "失败";
            //}
            //else if (name == "StateName_Scheduled")
            //{
            //    return "计划";
            //}
            //else if (name == "PropertyKey_CurrentCulture")
            //{
            //    return "当前编码";
            //}
            //else if (name == "PropertyKey_CurrentUICulture")
            //{
            //    return "当前界面编码";
            //}
            //else if (name == "PropertyKey_RecurringJobId")
            //{
            //    return "任务编号";
            //}
            //else if (name == "PropertyKey_RetryCount")
            //{
            //    return "异常重试次数";
            //}
            //else if (name == "Reason_Triggered by recurring job scheduler")
            //{
            //    return "由作业调度系统自动触发";
            //}
            //else if (name == "Reason_Triggered by DelayedJobScheduler")
            //{
            //    return "由作业调度系统延迟触发";
            //}
            //else if (name == "Reason_Triggered using recurring job manager")
            //{
            //    return "使用 仪表盘任务管理器 手动触发";
            //}
            //else if (name == "Reason_Triggered via Dashboard UI")
            //{
            //    return "仪表盘界面触发";
            //}
            //else if (name == "Reason_Continuation condition was not met")
            //{
            //    return "未满足继续条件";
            //}
            //else if (name == "Reason_An error occurred while deserializing the continuation")
            //{
            //    return "在反序列化时发生了一个解析错误";
            //}
            //else if (name == "Reason_An exception occurred during processing of a background job.")
            //{
            //    return "后台作业处理过程中出现异常。";
            //}
            //else if (name == "Background job '{0}' has expired or could not be found on the server.")
            //{
            //    return "后台作业“{0}”已过期或无法在服务器上找到。";
            //}
            //else if (name == "Queue:")
            //{
            //    return "队列:";
            //}
            //else if (name == "Queue")
            //{
            //    return "队列";
            //}
            //else if (name == "Enqueue at:")
            //{
            //    return "列队时间:";
            //}
            //else if (name == "Latency:")
            //{
            //    return "等待时间:";
            //}
            //else if (name == "Duration:")
            //{
            //    return "持续时间:";
            //}
            //else if (name == "Result:")
            //{
            //    return "结果:";
            //}
            //else if (name == "Server:")
            //{
            //    return "服务器:";
            //}
            //else if (name == "Worker:")
            //{
            //    return "工作:";
            //}
            //else if (name == "Parent")
            //{
            //    return "父级";
            //}
            //else if (name == "Next State")
            //{
            //    return "下一状态";
            //}
            //else if (name == "Options")
            //{
            //    return "选项";
            //}
            //else if (name == "d")
            //{
            //    return "天";
            //}
            //else if (name == "h")
            //{
            //    return "小时";
            //}
            //else if (name == "m")
            //{
            //    return "分钟";
            //}
            //else if (name == "s")
            //{
            //    return "秒";
            //}
            //else if (name == "ms")
            //{
            //    return "毫秒";
            //}
            //else if (name == "TimeZone")
            //{
            //    return "时区";
            //}
            //else if (name == "Display Name")
            //{
            //    return "显示名称";
            //}
            //else
            //if (System.Text.RegularExpressions.Regex.IsMatch(name, "Reason_Background job has exceeded latency timeout of (?<timeoutInSeconds>\\d+) second(s)"))
            //{
            //    var timeoutInSeconds = System.Text.RegularExpressions.Regex.Match(name, "Reason_Background job has exceeded latency timeout of (?<timeoutInSeconds>\\d+) second(s)").Groups["timeoutInSeconds"].Value;
            //    return $"后台作业已等待{timeoutInSeconds}秒,超过超时时间。";
            //}
            //else if (System.Text.RegularExpressions.Regex.IsMatch(name, "Reason_Can not change the state to '(?<StateName>.*)': target method was not found."))
            //{
            //    var stateName = System.Text.RegularExpressions.Regex.Match(name, "Reason_Can not change the state to '(?<StateName>.*)': target method was not found.").Groups["StateName"].Value;
            //    var stateName2 = this.GetString("StateName_" + stateName);
            //    return $"不能改变状态为“{stateName2}”：找不到目标方法。";
            //}
            //else if (System.Text.RegularExpressions.Regex.IsMatch(name, "Reason_Retry attempt (?<retryAttempt>\\d+) of (?<Attempts>\\d+): (?<exceptionMessage>.*)"))
            //{
            //    var regexMatch = System.Text.RegularExpressions.Regex.Match(name, "Reason_Retry attempt (?<RetryAttempt>\\d+) of (?<Attempts>\\d+): (?<ExceptionMessage>.*)");
            //    var retryAttempt = regexMatch.Groups["RetryAttempt"].Value;
            //    var attempts = regexMatch.Groups["Attempts"].Value;
            //    var exceptionMessage = regexMatch.Groups["ExceptionMessage"].Value;

            //    return $"重新尝试执行 {retryAttempt} / {attempts}：{exceptionMessage}";
            //}
            //else
            {
                var str = _resourceManager.GetString(name, culture) ?? name;
                if (IsHasEnglish(str))
                {
                }
                return str;
            }
            ////var r = System.Web.HttpContext.Current.Request;
            //var sss = base.GetString(name, culture);
            //dddddd.GetOrAdd(name, sss);
            //return sss;
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsHasChinese(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\u4e00-\u9fa5]+$");
            //return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[^\x00-\xFF]");
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsHasEnglish(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z]");
        }
    }
}