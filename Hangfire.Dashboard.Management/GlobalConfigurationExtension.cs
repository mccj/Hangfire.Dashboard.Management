using Hangfire.Dashboard.Extensions;
using Hangfire.Dashboard.Management.Pages;
using Hangfire.Dashboard.Management.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 翻译
/// //是否启用暂停功能
/// //是否启用添加功能
/// 添加任务
/// </summary>
namespace Hangfire.Dashboard.Management
{
    public static class GlobalConfigurationExtension
    {
        /// <summary>
        /// 加载可管理任务
        /// </summary>
        /// <param name="config"></param>
        /// <param name="assembly"></param>
        public static IGlobalConfiguration UseManagementPages(this IGlobalConfiguration config, params Assembly[] assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            UseManagementPages(config, cc => cc.AddJobs(assembly));
            return config;
        }
        /// <summary>
        /// 加载可管理任务
        /// </summary>
        /// <param name="config"></param>
        /// <param name="types"></param>
        public static IGlobalConfiguration UseManagementPages(this IGlobalConfiguration config, params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            UseManagementPages(config, cc => cc.AddJobs(types));
            return config;
        }
        /// <summary>
        /// 加载可管理任务
        /// </summary>
        /// <param name="config"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IGlobalConfiguration UseManagementPages(this IGlobalConfiguration config, Func<ManagementPagesOptions, ManagementPagesOptions> func)
        {
            var options = func(new ManagementPagesOptions());
            CreateManagement(options);
            return config;
        }
        private static void CreateManagement(ManagementPagesOptions options = null)
        {
            #region 翻译
            var resourceManager = Hangfire.Dashboard.Resources.Strings.ResourceManager;
            var resourceManField = typeof(Hangfire.Dashboard.Resources.Strings).GetField("resourceMan", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var customHangfireLanguage = new CustomHangfireLanguage(resourceManager, options.TranslateFunc);
            resourceManField.SetValue(null, customHangfireLanguage /*_customHangfireLanguage*/);
            JobHistoryRenderer.Register(customHangfireLanguage);
            //翻译时间脚本
            var jsPath = Hangfire.Dashboard.DashboardRoutes.Routes.Contains("/js[0-9]+") ? "/js[0-9]+" : "/js[0-9]{3}";
            //DashboardRoutes.Routes.Append(jsPath, new DynamicJsDispatcher());
            DashboardRoutes.Routes.Append(jsPath, new EmbeddedResourceDispatcher("application/javascript", Assembly.GetExecutingAssembly(), $"{typeof(GlobalConfigurationExtension).Namespace}.Content.momentLocale.js"));
            //
            #endregion 翻译
            //Cron最近5次运行时间
            //cron?cron=0+0+0+*+*+%3F+
            DashboardRoutes.Routes.Add("/cron", new CommandDispatcher(context =>
            {
                var cron = context.Request.GetQuery("cron");
                //var result = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cron, new Options
                //{
                //    ThrowExceptionOnParseError = false,
                //    Verbose = false,
                //    DayOfWeekStartIndexZero = true,
                //    Locale = (locale ?? "en-US")
                //});
                //var cronDescription = Hangfire.Cron.GetDescription(cron);
                var cronDescription = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cron);
                var cronSchedule = NCrontab.CrontabSchedule.Parse(cron);
                var example = cronSchedule.GetNextOccurrences(System.DateTime.UtcNow, System.DateTime.Now.AddYears(5)).Take(5).ToArray();
                var str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    Description = cronDescription,
                    Example = example
                });
                context.Response.WriteAsync(str);
                return true;
            }));
            var pages = options.GetPages();
            #region 任务
            ManagementSidebarMenu.Items.Clear();
            foreach (var pageInfo in pages)
            {
                //添加命令
                //ManagementBasePage.AddCommands(pageInfo.Queue);
                //添加菜单连接
                var path = $"{ManagementPage.UrlRoute}/{pageInfo.Title.ToBase64Url()}";
                ManagementSidebarMenu.Items.Add(p => new MenuItem(pageInfo.Title, path)
                {
                    Active = p.RequestPath.StartsWith(path)
                });
                ////添加页面
                DashboardRoutes.Routes.AddRazorPage(path, x => new ManagementBasePage(pageInfo.Title, pageInfo.Title, pageInfo.Pages));
            }
            #endregion 任务

            //暂停取消功能
            GlobalJobFilters.Filters.Add(new PauseStateAttribute());
            DashboardRoutes.Routes.AddRazorPage(ManagementPage.UrlRoute, x => new ManagementPage());
            //DashboardRoutes.Routes.AddRazorPage("/recurring/addJobs", x => new ManagementPage());

            // can't use the method of Hangfire.Console as it's usage overrides any similar usage here. Thus
            // we have to add our own endpoint to load it and call it from our code. Actually is a lot less work
            //DashboardRoutes.Routes.Add("/jsm", new EmbeddedResourceDispatcher("application/javascript", Assembly.GetExecutingAssembly(), $"{typeof(GlobalConfigurationExtension).Namespace}.Content.management.js"));
            //DashboardRoutes.Routes.Add("/jsm", new CombinedResourceDispatcher("application/javascript", Assembly.GetExecutingAssembly(), $"{typeof(GlobalConfigurationExtension).Namespace}.Content", new[] { "management.js", "cron.js" }));
            DashboardRoutes.Routes.Append(jsPath, new CombinedResourceDispatcher("application/javascript", Assembly.GetExecutingAssembly(), $"{typeof(GlobalConfigurationExtension).Namespace}.Content", new[] { "management.js", "cron.js" }));

            //NavigationMenu.Items.Add(page => new MenuItem(ManagementPage.Title, page.Url.To(ManagementPage.UrlRoute))
            //{
            //    Active = page.RequestPath.StartsWith(ManagementPage.UrlRoute)
            //});

            //执行暂停命令
            DashboardRoutes.Routes.AddBatchCommand("/recurring/pause", (context, jobId) =>
                 {
                     using (var connection = context.Storage.GetConnection())
                     {
                         connection.SetPauseState(jobId, true);
                     }
                 });
            //执行恢复命令
            DashboardRoutes.Routes.AddBatchCommand("/recurring/repeat", (context, jobId) =>
                 {
                     using (var connection = context.Storage.GetConnection())
                     {
                         connection.SetPauseState(jobId, false);
                     }
                 });
            //执行添加任务
            DashboardRoutes.Routes.AddCommand($"{ManagementPage.UrlRoute}/addJob", context =>
            {
                if (context.Request.Method == "POST")
                {
                    //Hangfire官方在Owin模式获取参数会有问题，只能获取一次，第二次会是空值
                    var getFormValue = new Func<string, IList<string>>(key =>
                   {
                       ////Owin模式
                       //var environment = context.GetOwinEnvironment();
                       //var _context = new Microsoft.Owin.OwinContext(environment);
                       //var form = _context.Request.ReadFormSafeAsync().Result;
                       //var r =  form.GetValues(_key);
                       ////aspnet模式
                       //var r = context.Request.GetFormValuesAsync(key)?.Result;
                       //return r.Select(f => string.IsNullOrWhiteSpace(f) ? null : f).ToArray();
                       //通用模式
                       IList<string> getValue(string _key)
                       {
#if NETFULL
                           //Owin模式
                           var environment = context.GetOwinEnvironment();
                           var _context = new Microsoft.Owin.OwinContext(environment);
                           var form = _context.Request?.ReadFormAsync()?.Result;
                           return form?.GetValues(_key);
#else
                           return context.Request.GetFormValuesAsync(_key)?.Result;
#endif
                       }
                       var r = getValue(key);
                       return r.Select(f => string.IsNullOrWhiteSpace(f) ? null : f).ToArray();
                   });

                    var jobtype = getFormValue("type")?.FirstOrDefault();
                    var id = getFormValue("id")?.FirstOrDefault();
                    var jobMetadata = pages.SelectMany(f => f.Pages.SelectMany(ff => ff.Metadatas.Where(fff => fff.GetId() == id))).FirstOrDefault();
                    var par = new List<object>();

                    foreach (var parameterInfo in jobMetadata.Parameters)
                    {
                        if (parameterInfo.ParameterType == typeof(Server.PerformContext) || parameterInfo.ParameterType == typeof(IJobCancellationToken))
                        {
                            par.Add(null);
                            continue;
                        };

                        var variable = $"{id}_{parameterInfo.Name}";
                        if (parameterInfo.ParameterType == typeof(DateTime))
                        {
                            variable = $"{variable}_datetimepicker";
                        }

                        var t = getFormValue(variable);

                        object item = null;
                        var formInput = t?.FirstOrDefault();
                        if (parameterInfo.ParameterType == typeof(string))
                        {
                            item = formInput;
                        }
                        else if (parameterInfo.ParameterType == typeof(int))
                        {
                            if (formInput != null) item = int.Parse(formInput);
                        }
                        else if (parameterInfo.ParameterType == typeof(DateTime))
                        {
                            item = formInput == null ? DateTime.MinValue : DateTime.Parse(formInput);
                        }
                        else if (parameterInfo.ParameterType == typeof(bool))
                        {
                            item = formInput == "on";
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        par.Add(item);
                    }


                    var job = new Common.Job(jobMetadata.Type, jobMetadata.MethodInfo, par.ToArray());
                    var client = new BackgroundJobClient(context.Storage);
                    switch (jobtype)
                    {
                        case "CronExpression":
                            {
                                var manager = new RecurringJobManager(context.Storage);
                                try
                                {
                                    var queue = getFormValue($"{id}_sys_queue")?.FirstOrDefault();
                                    var timeZone = getFormValue($"{id}_sys_timeZone")?.FirstOrDefault() ?? "Utc";
                                    var displayName = getFormValue($"{id}_sys_displayName")?.FirstOrDefault();

                                    var jobQueue = (queue?.Trim().Replace("-", "_").Replace(" ", "_") ?? jobMetadata.Queue)?.ToLower();
                                    var jobTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone?.Trim()) ?? TimeZoneInfo.Utc;
                                    var jobDisplayName = string.IsNullOrWhiteSpace(displayName) ? jobMetadata.DisplayName : displayName;

                                    var schedule = getFormValue("schedule")?.FirstOrDefault();
                                    var cron = getFormValue($"{id}_sys_cron")?.FirstOrDefault();

                                    if (string.IsNullOrWhiteSpace(schedule ?? cron)) { throw new Exception("请填写 Cron 表达式"); }
                                    manager.AddOrUpdate(jobDisplayName, job, schedule ?? cron, jobTimeZone, jobQueue);
                                }
                                catch (Exception)
                                {
                                    return false;
                                }
                                return true;
                                //break;
                            }
                        case "ScheduleDateTime":
                            {
                                var datetime = getFormValue($"{id}_sys_datetime")?.FirstOrDefault();
                                if (string.IsNullOrWhiteSpace(datetime))
                                {
                                    //context.Response.StatusCode = 400;
                                    ////Hangfire.Dashboard.AspNetCoreDashboardResponse
                                    context.Response.WriteAsync("请填写 执行时间 表达式");
                                    throw new Exception("请填写 执行时间 表达式");
                                }
                                var jobId = client.Create(job, new States.ScheduledState(DateTime.Parse(datetime)));//Queue
                                return jobId != string.Empty;
                                //break;
                            }
                        case "ScheduleTimeSpan":
                            {
                                var schedule = getFormValue("schedule")?.FirstOrDefault();
                                var timeSpan = getFormValue($"{id}_sys_timespan")?.FirstOrDefault();
                                if (string.IsNullOrWhiteSpace(schedule ?? timeSpan)) { throw new Exception("请填写 延迟时间 表达式"); }
                                var jobId = client.Create(job, new States.ScheduledState(TimeSpan.Parse(schedule ?? timeSpan)));//Queue
                                return jobId != string.Empty;
                                //break;
                            }
                        //case "ContinueWith":
                        //    {
                        //        var parentId = getValue("{id}_sys_parentId")?.FirstOrDefault();
                        //        var optionsStr = getValue($"{id}_sys_JobContinuationOptions")?.FirstOrDefault();

                        //        var jobContinuationOptions = JobContinuationOptions.OnlyOnSucceededState;
                        //        Enum.TryParse<JobContinuationOptions>(optionsStr, out jobContinuationOptions);
                        //        var jobId = client.Create(job, new States.AwaitingState(parentId, null, jobContinuationOptions));
                        //        return jobId != string.Empty;
                        //        //break;
                        //    }
                        case "Enqueue":
                        default:
                            {
                                var queue = getFormValue($"{id}_sys_queue")?.FirstOrDefault();
                                var jobQueue = (queue?.Trim().Replace("-", "_").Replace(" ", "_") ?? jobMetadata.Queue)?.ToLower();
                                var jobId = client.Create(job, new States.EnqueuedState(jobQueue));
                                return jobId != string.Empty;
                                //break;
                            }
                            //break;
                    }

                    /*
                 EnqueuedState
                    Queue
                 ScheduledState
                 AddOrUpdate
                    DisplayName
                    TimeZone
                    Queue
                 ContinueWith
                    parentId
                    jobContinuationOptions
                 */



                    //if (!string.IsNullOrEmpty(schedule))
                    //{
                    //    var minutes = int.Parse(schedule);
                    //    return client.Create(job, new ScheduledState(new TimeSpan(0, 0, minutes, 0))) != string.Empty;
                    //}
                    //else if (!string.IsNullOrEmpty(cron))
                    //{
                    //    var manager = new RecurringJobManager(context.Storage);
                    //    try
                    //    {
                    //        manager.AddOrUpdate(jobMetadata.DisplayName, job, cron, TimeZoneInfo.Utc, queue);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        return false;
                    //    }
                    //    return true;
                    //}
                }
                return false;
            });
            //替换页面
            var type = Type.GetType("Hangfire.Dashboard.RazorPageDispatcher," + typeof(IGlobalConfiguration).Assembly.FullName);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type, new object[] { new Func<System.Text.RegularExpressions.Match, RazorPage>(x => new Hangfire.Dashboard.Management.Pages.RecurringJobsPage()) });
                var dispatcher = obj as IDashboardDispatcher;//new RazorPageDispatcher( )
                Hangfire.Dashboard.DashboardRoutes.Routes.Replace("/recurring", dispatcher);

                object obj2 = Activator.CreateInstance(type, new object[] { new Func<System.Text.RegularExpressions.Match, RazorPage>(x => new Hangfire.Dashboard.Management.Pages.JobDetailsPage(x.Groups["JobId"].Value)) });
                var dispatcher2 = obj2 as IDashboardDispatcher;//new RazorPageDispatcher( )
                Hangfire.Dashboard.DashboardRoutes.Routes.Replace("/jobs/details/(?<JobId>.+)", dispatcher2);

                object obj3 = Activator.CreateInstance(type, new object[] { new Func<System.Text.RegularExpressions.Match, RazorPage>(x => new Hangfire.Dashboard.Management.Pages.RetriesPage()) });
                var dispatcher3 = obj3 as IDashboardDispatcher;//new RazorPageDispatcher( )
                Hangfire.Dashboard.DashboardRoutes.Routes.Replace("/retries", dispatcher3);

                object obj4 = Activator.CreateInstance(type, new object[] { new Func<System.Text.RegularExpressions.Match, RazorPage>(x => new Hangfire.Dashboard.Management.Pages.SucceededJobs()) });
                var dispatcher4 = obj4 as IDashboardDispatcher;//new RazorPageDispatcher( )
                Hangfire.Dashboard.DashboardRoutes.Routes.Replace("/jobs/succeeded", dispatcher4);
            }
        }
    }

    public class ManagementPagesOptions
    {
        public ManagementPagesOptions()
        {
        }
        protected internal Func<string, System.Globalization.CultureInfo, string> TranslateFunc { get; private set; }
        private List<ManagePage> Pages { get; /*set; */} = new List<ManagePage>();

        protected internal aaaa[] GetPages()
        {
            return Pages.GroupBy(f => f.Title.IsNullOrWhiteSpace() ? string.Empty : f.Title.Trim()).Select(f => new aaaa { Title = f.Key, Pages = f.Where(ff => ff.Metadatas.Length > 0).ToArray() }).ToArray();
        }
        protected internal class aaaa
        {
            public string Title { get; set; }
            public ManagePage[] Pages { get; set; }
            //public string MenuName { get; internal set; }
        }

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public ManagementPagesOptions Translate(Func<string, System.Globalization.CultureInfo, string> func)
        {
            this.TranslateFunc = func;
            return this;
        }
        /// <summary>
        /// 加载可管理任务
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public ManagementPagesOptions AddJobs(params Assembly[] assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            return this.AddJobs(assembly.SelectMany(f => f.GetTypes()).ToArray());
        }
        public ManagementPagesOptions AddJobs(Func<Type[]> func)
        {
            return AddJobs(func());
        }
        /// <summary>
        /// 加载可管理任务
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public ManagementPagesOptions AddJobs(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            return AddJobs(() =>
             {
                 return types
                 //.Where(x =>/* x.IsInterface && */typeof(IJob).IsAssignableFrom(x) && x.Name != (typeof(IJob).Name))
                 .Select(ti => new { Type = ti, ManagePage = ti.GetCustomAttribute<Metadata.ManagementPageAttribute>() ?? new Metadata.ManagementPageAttribute("默认", States.EnqueuedState.DefaultQueue) })
                 .Where(ti => ti != null && ti.ManagePage != null)
                 .Select(ti =>
                    new ManagePage(ti.ManagePage.Title, /*ti.ManagePage.MenuName, getQueue(ti.ManagePage.Queue),*/
                        ti.Type.GetMethods()
                        .Where(methodInfo => methodInfo.GetCustomAttribute<JobAttribute>() != null)
                        .Select(methodInfo => toJobMetadata(methodInfo, queue: ti.ManagePage.Queue))
                        .Where(f => !string.IsNullOrWhiteSpace(f.DisplayName)).ToArray()))
                        .Where(f => f.Metadatas.Length > 0);
             });
        }
        public ManagementPagesOptions AddJobs(Func<IEnumerable<ManagePage>> typesProvider)
        {
            if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));
            Pages.AddRange(typesProvider());
            return this;
        }
        public ManagementPagesOptions AddJobs(Func<Metadata.JobMetadata> methodCall, string title = null)
        {
            Pages.Add(new ManagePage(title, new[] { methodCall() }));
            return this;
        }
        private Metadata.JobMetadata toJobMetadata(MethodInfo tm, object[] defaultValue = null, string recurringJobId = null, string queue = null)
        {
            var _defaultValue = new System.Collections.Queue(defaultValue ?? new object[] { });
            var getQueue = new Func<string, string>(_queue => string.IsNullOrWhiteSpace(_queue) ? States.EnqueuedState.DefaultQueue : _queue);
            return new Metadata.JobMetadata
            {
                Type = tm.ReflectedType,
                Queue = getQueue(queue),
                MethodInfo = tm,
                Parameters = tm.GetParameters()
                      //.Where(parameterInfo => parameterInfo.ParameterType != typeof(Server.PerformContext) && parameterInfo.ParameterType != typeof(IJobCancellationToken))
                      .Select(f => new
                      {
                          Parameter = f,
                          DisplayData = f.GetCustomAttribute<Metadata.DisplayDataAttribute>(true)
                      })
                      .Select(f => new Metadata.JobParameter
                      {
                          Name = f?.Parameter?.Name,
                          ParameterType = f?.Parameter?.ParameterType,
                          LabelText = f?.DisplayData?.LabelText ?? f?.Parameter?.Name,
                          PlaceholderText = f?.DisplayData?.PlaceholderText ?? f?.Parameter?.Name,
                          DefaultValue = _defaultValue.Count > 0 ? _defaultValue.Dequeue() : null
                      }).ToArray(),
                Description = tm.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? tm.Name,
                DisplayName = tm.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>()?.DisplayName ?? recurringJobId
            };
        }
        public ManagementPagesOptions AddJobs(MethodInfo methodInfo, object[] defaultValue = null, string title = null, string recurringJobId = null, string queue = "default")
        {
            return AddJobs(() => toJobMetadata(methodInfo, defaultValue: defaultValue, recurringJobId: recurringJobId ?? methodInfo.Name, queue: queue), title);
        }
        public ManagementPagesOptions AddJobs(Expression<Action> methodCall, string title = null, string recurringJobId = null, string queue = "default")
        {
            var expression = methodCall.Body as System.Linq.Expressions.MethodCallExpression;
            var parameters = expression.Arguments.OfType<ConstantExpression>().Select(f => f.Value).ToArray();
            return AddJobs(expression.Method, title: title, defaultValue: parameters, recurringJobId: recurringJobId, queue: queue);
        }
        public ManagementPagesOptions AddJobs<T>(Expression<Func<T, Task>> methodCall, string title = null, string recurringJobId = null, string queue = "default")
        {
            var expression = methodCall.Body as System.Linq.Expressions.MethodCallExpression;
            return AddJobs(expression.Method, title: title, recurringJobId: recurringJobId, queue: queue);
        }
        public ManagementPagesOptions AddJobs(Expression<Func<Task>> methodCall, string title = null, string recurringJobId = null, string queue = "default")
        {
            var expression = methodCall.Body as System.Linq.Expressions.MethodCallExpression;
            return AddJobs(expression.Method, title: title, recurringJobId: recurringJobId, queue: queue);
        }

        //public SqlServerStorageOptions RemoveIfExists(string recurringJobId)
        //{
        //    return this;
        //}
    }
    ///// <summary>
    ///// Hangfire官方在Owin模式获取参数会有问题，只能获取一次，第二次会是空值
    ///// </summary>
    //public class HangfireDashboardRequest : DashboardRequest
    //{
    //    private readonly DashboardRequest _request = null;
    //    /// <summary>
    //    /// Hangfire官方在Owin模式获取参数会有问题，只能获取一次，第二次会是空值
    //    /// </summary>
    //    /// <param name="response"></param>
    //    public HangfireDashboardRequest(DashboardRequest request)
    //    {
    //        _request = request;
    //    }
    //    public override string Method => _request.Method;

    //    public override string Path => _request.Path;

    //    public override string PathBase => _request.PathBase;

    //    public override string LocalIpAddress => _request.LocalIpAddress;

    //    public override string RemoteIpAddress => _request.RemoteIpAddress;

    //    public override Task<IList<string>> GetFormValuesAsync(string key)
    //    {
    //        var type = _request.GetType();
    //        if (type.Name == "OwinDashboardRequest")
    //        {
    //            return System.Threading.Tasks.Task.Run<IList<string>>(() =>
    //            {//Owin 模式
    //                var owinContext = type.GetField("_context", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_request);
    //                var owinContextType = owinContext.GetType();
    //                var request = owinContextType.GetProperty("Request").GetValue(owinContext);
    //                var requestType = request.GetType();
    //                var formTask = requestType.GetMethod("ReadFormAsync").Invoke(request, null) as Task;//Task<IFormCollection>
    //                var formTaskType = formTask.GetType();
    //                var formCollection = formTaskType.GetProperty("Result").GetValue(formTask);
    //                var formCollectionType = formCollection.GetType();

    //                var from = formCollectionType.GetMethod("GetValues").Invoke(formCollection, new[] { key }) as IList<string>;

    //                return from;

    //                //Hangfire官方在Owin模式获取参数会有问题，只能获取一次，第二次会是空值
    //                //var environment = context.GetOwinEnvironment();
    //                //var _context = new Microsoft.Owin.OwinContext(environment);
    //                //var form = _context.Request.ReadFormSafeAsync().Result;
    //                //return form.GetValues(key);
    //            });
    //        }
    //        else
    //        {
    //            return _request.GetFormValuesAsync(key);
    //        }
    //    }

    //    public override string GetQuery(string key)
    //    {
    //        return _request.GetQuery(key);
    //    }
    //}
}
