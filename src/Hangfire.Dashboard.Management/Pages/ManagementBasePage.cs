using Hangfire.Server;
using System;
using System.Linq;

namespace Hangfire.Dashboard.Management.Pages
{
    internal partial class ManagementBasePage : RazorPage
    {
        private readonly string pageTitle;
        private readonly string pageHeader;
        private readonly Metadata.JobMetadata[] managePages;

        protected internal ManagementBasePage(string pageTitle, string pageHeader, Metadata.JobMetadata[] managePages)
        {
            this.pageTitle = pageTitle;
            this.pageHeader = pageHeader;
            this.managePages = managePages;
        }

        public void WriteLiteralTo(System.IO.TextWriter writer, string value)
        {
            if (!string.IsNullOrEmpty(value))
                writer.Write(value);
        }

        public void WriteTo(System.IO.TextWriter writer, object value)
        {
            if (value == null)
                return;
            var html = value as NonEscapedString;
            WriteLiteralTo(writer, html?.ToString() ?? Encode(value.ToString()));
        }

        private string Encode(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : System.Net.WebUtility.HtmlEncode(text);
        }

        public NonEscapedString Raw(string text)
        {
            return new NonEscapedString(text);
        }

        protected virtual void Content()
        {
            //var jobs = JobsHelper.Metadata.Where(j => j.Queue.Contains(queue));
            foreach (var jobMetadata in this.managePages)
            {
                if (jobMetadata == null) continue;
                //var route = $"{ManagementPage.UrlRoute}/{queue}/{jobMetadata.DisplayName.Replace(" ", string.Empty)}";
                //var id = $"{jobMetadata.DisplayName.Replace(" ", string.Empty)}";

                var id = $"{jobMetadata.GetId()/*(jobMetadata.Type.FullName + jobMetadata.MethodInfo.Name).ToBase64Url()*/}";
                var route = $"{ManagementPage.UrlRoute}/addJob";

                string inputs = string.Empty;
                if (jobMetadata.Parameters?.Length > 0)
                {
                    foreach (var parameterInfo in jobMetadata.Parameters)
                    {
                        if (parameterInfo.ParameterType == typeof(PerformContext) || parameterInfo.ParameterType == typeof(IJobCancellationToken))
                            continue;

                        //DisplayDataAttribute displayInfo = parameterInfo.GetCustomAttribute<DisplayDataAttribute>(true);

                        var parameterType = parameterInfo.ParameterType;
                        if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            parameterType = parameterType.GetGenericArguments()[0];
                        }

                        var myId = $"{id}_{parameterInfo.Name}";
                        if (parameterType == typeof(string) && parameterInfo.ConvertType == null)
                        {
                            //inputs += InputTextbox(myId, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo.Name);
                            inputs += Input(myId, parameterInfo?.CssClasses, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.DescriptionText, parameterInfo?.IsMultiLine == true ? "textarea" : "text", parameterInfo.DefaultValue, parameterInfo?.InputMask, parameterInfo?.Required == true, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterType == typeof(int))
                        {
                            //inputs += InputNumberbox(myId, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo.Name);
                            inputs += Input(myId, parameterInfo?.CssClasses, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.DescriptionText, "number", parameterInfo.DefaultValue, parameterInfo?.InputMask, parameterInfo?.Required == true, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterType == typeof(Uri))
                        {
                            //inputs += InputNumberbox(myId, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo.Name);
                            inputs += Input(myId, parameterInfo?.CssClasses, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.DescriptionText, "url", parameterInfo.DefaultValue, parameterInfo?.InputMask, parameterInfo?.Required == true, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterType == typeof(DateTime) || parameterType == typeof(DateTimeOffset))
                        {
                            inputs += InputDatebox(myId, parameterInfo?.CssClasses, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo.DefaultValue, parameterInfo?.Required == true, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterType == typeof(bool))
                        {
                            inputs += "<br/>" + InputCheckbox(myId, parameterInfo?.CssClasses, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo.Name, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterType.IsEnum)
                        {
                            var data = Enum.GetNames(parameterType).ToDictionary(f => f, f => f).ToArray();
                            inputs += InputDataList(myId, string.Empty, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, data, parameterInfo.DefaultValue?.ToString(), parameterInfo?.IsMultiLine == true, parameterInfo?.InputMask, parameterInfo?.Required == true, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else if (parameterInfo.ConvertType != null && typeof(Metadata.IInputDataList).IsAssignableFrom(parameterInfo.ConvertType))
                        {
                            var r = System.Activator.CreateInstance(parameterInfo.ConvertType) as Metadata.IInputDataList;
                            var data = r.GetData().ToArray();
                            var defaultValue = r.GetDefaultValue();
                            inputs += InputDataList(myId, string.Empty, parameterInfo?.LabelText ?? parameterInfo.Name, parameterInfo?.PlaceholderText ?? parameterInfo?.LabelText ?? parameterInfo.Name, data, defaultValue ?? parameterInfo.DefaultValue?.ToString(), parameterInfo?.IsMultiLine == true, parameterInfo?.InputMask, parameterInfo?.Readonly == true).ToHtmlString();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(inputs))
                {
                    inputs += "<hr class=\"wide\">";
                }
                var options = string.Empty;

                options += Input($"{id}_sys_queue", "commands-options Enqueue CronExpression", Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("Queue"), Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("Queue"), "", "text", jobMetadata.Queue, "", false, jobMetadata.DisabledQueueSetting == true).ToHtmlString();
                options += InputDataList($"{id}_sys_timeZone", "commands-options CronExpression", Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("TimeZone"), Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("TimeZone"), TimeZoneInfo.GetSystemTimeZones().ToDictionary(f => f.Id, f => f.DisplayName).ToArray(), TimeZoneInfo.Local.Id).ToHtmlString();
                options += Input($"{id}_sys_displayName", "commands-options CronExpression", Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("Display Name"), Hangfire.Dashboard.Resources.Strings.ResourceManager.GetString("Display Name"), "", "text", "").ToHtmlString();
                //options += Input($"{id}_sys_parentId", string.Empty, "Parent Job Id", "Parent Job Id", "text", "").ToHtmlString();
                //options += InputDataList($"{id}_sys_parentOption", string.Empty, "Parent Job Option", "Parent Job Option", new[] { "None" }.Concat(Enum.GetValues(typeof(JobContinuationOptions)).OfType<JobContinuationOptions>().Select(f => f.ToString())).ToDictionary(f => f, f => f).ToArray(), JobContinuationOptions.OnlyOnSucceededState).ToHtmlString();

                var buttons = CreateButtons(route, "入队", "入队中...", id).ToHtmlString();

                var isFirst = this.managePages.First() == jobMetadata;
                var jobSnippetCode = MissionRenderer.RenderMission(jobMetadata.MethodInfo);
                WriteLiteral(Panel(id, jobMetadata.DisplayName, jobMetadata.Description, jobMetadata.HideJobSnippetCode == true ? null : jobSnippetCode, inputs, options, buttons, isFirst).ToHtmlString());
            }
            //WriteLiteral("\r\n<script src=\"");
            //Write(Url.To($"/jsm"));
            //WriteLiteral("\"></script>\r\n");
        }

        //public static void AddCommands(string queue)
        //{
        //    var jobs = JobsHelper.Metadata.Where(j => j.Queue.Contains(queue));

        //    foreach (var jobMetadata in jobs)
        //    {
        //        var route = $"{ManagementPage.UrlRoute}/{queue}/{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jobMetadata.Type.FullName))/*DisplayName.Replace(" ", string.Empty)*/}";

        //        DashboardRoutes.Routes.AddCommand(route, context =>
        //    {
        //        var par = new List<object>();
        //        var schedule = Task.Run(() => context.Request.GetFormValuesAsync($"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jobMetadata.Type.FullName))/*jobMetadata.DisplayName.Replace(" ", string.Empty)*/}_schedule")).Result.FirstOrDefault();
        //        var cron = Task.Run(() => context.Request.GetFormValuesAsync($"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jobMetadata.Type.FullName))/*jobMetadata.DisplayName.Replace(" ", string.Empty)*/}_cron")).Result.FirstOrDefault();

        //        foreach (var parameterInfo in jobMetadata.MethodInfo.GetParameters())
        //        {
        //            if (parameterInfo.ParameterType == typeof(PerformContext) || parameterInfo.ParameterType == typeof(IJobCancellationToken))
        //            {
        //                par.Add(null);
        //                continue;
        //            };

        //            var variable = $"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jobMetadata.Type.FullName))/*jobMetadata.DisplayName.Replace(" ", string.Empty)*/}_{parameterInfo.Name}";
        //            if (parameterInfo.ParameterType == typeof(DateTime))
        //            {
        //                variable = $"{variable}_datetimepicker";
        //            }

        //            var t = Task.Run(() => context.Request.GetFormValuesAsync(variable)).Result;

        //            object item = null;
        //            var formInput = t.FirstOrDefault();
        //            if (parameterInfo.ParameterType == typeof(string))
        //            {
        //                item = formInput;
        //            }
        //            else if (parameterInfo.ParameterType == typeof(int))
        //            {
        //                if (formInput != null) item = int.Parse(formInput);
        //            }
        //            else if (parameterInfo.ParameterType == typeof(DateTime))
        //            {
        //                item = formInput == null ? DateTime.MinValue : DateTime.Parse(formInput);
        //            }
        //            else if (parameterInfo.ParameterType == typeof(bool))
        //            {
        //                item = formInput == "on";
        //            }
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }

        //            par.Add(item);

        //        }

        //        var job = new Job(jobMetadata.Type, jobMetadata.MethodInfo, par.ToArray());

        //        var client = new BackgroundJobClient(context.Storage);

        //        if (!string.IsNullOrEmpty(schedule))
        //        {
        //            var minutes = int.Parse(schedule);
        //            return client.Create(job, new ScheduledState(new TimeSpan(0, 0, minutes, 0))) != string.Empty;
        //        }
        //        else if (!string.IsNullOrEmpty(cron))
        //        {
        //            var manager = new RecurringJobManager(context.Storage);
        //            try
        //            {
        //                manager.AddOrUpdate(jobMetadata.DisplayName, job, cron, TimeZoneInfo.Utc, queue);
        //            }
        //            catch (Exception)
        //            {
        //                return false;
        //            }
        //            return true;
        //        }

        //        var jobId = client.Create(job, new EnqueuedState(jobMetadata.Queue));
        //        return jobId != string.Empty;
        //    });
        //    }
        //}
        //public override void Execute()
        //{
        //    WriteLiteral("\r\n");
        //    Layout = new LayoutPage(pageTitle);

        //    WriteLiteral("<div class=\"row\">\r\n");
        //    WriteLiteral("<div class=\"col-md-3\">\r\n");

        //    Write(Html.RenderPartial(new CustomSidebarMenu(ManagementSidebarMenu.Items)));

        //    WriteLiteral("</div>\r\n");
        //    WriteLiteral("<div class=\"col-md-9\">\r\n");
        //    WriteLiteral("<h1 class=\"page-header\">\r\n");
        //    Write(pageHeader);
        //    WriteLiteral("</h1>\r\n");

        //    Content();

        //    WriteLiteral("\r\n</div>\r\n");
        //    WriteLiteral("\r\n</div>\r\n");
        //}

        //protected void Panel(string id, string heading, string description, string content, string buttons)
        //{
        //    WriteLiteral($@"<div class=""panel panel-default js-management"">
        //                      <div class=""panel-heading"">{heading}</div>
        //                      <div class=""panel-body"">
        //                        <p>{description}</p>
        //                      </div>
        //                      <div class=""panel-body"">");

        //    if (!string.IsNullOrEmpty(content))
        //    {
        //        WriteLiteral($@"<div class=""well"">
        //                            { content}
        //                        </div>

        //                      ");
        //    }

        //    WriteLiteral($@"<div id=""{id}_error"" ></div>
        //                    </div>
        //                    <div class=""panel-footer clearfix "">
        //                        <div class=""pull-right"">
        //                            { buttons}
        //                        </div>
        //                      </div>
        //                    </div>");
        //}

        //protected string CreateButtons(string url, string text, string loadingText, string id)
        //{
        //    return $@"

        //                <div class=""col-sm-2 pull-right"">
        //                    <button class=""js-management-input-commands btn btn-sm btn-success""
        //                            data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"" input-id=""{id}"">
        //                        <span class=""glyphicon glyphicon-play-circle""></span>
        //                        &nbsp;列队
        //                    </button>
        //                </div>
        //                <div class=""btn-group col-3 pull-right"">
        //                    <button type=""button"" class=""btn btn-info btn-sm dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
        //                        延时 &nbsp;
        //                        <span class=""caret""></span>
        //                    </button>

        //                    <ul class=""dropdown-menu"">
        //                        <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" schedule=""5""
        //                            data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">5 秒</a></li>
        //                        <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" schedule=""10""
        //                             data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">10 秒</a></li>
        //                        <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" schedule=""15""
        //                             data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">15 秒</a></li>
        //                        <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" schedule=""30""
        //                             data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">30 秒</a></li>
        //                        <li><a href=""#"" class=""js-management-input-commands"" input-id=""{id}"" schedule=""60""
        //                             data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">60 秒</a></li>

        //                    </ul>
        //                </div>

        //                <div class=""col-sm-5 pull-right"">
        //                    <div class=""input-group input-group-sm"">
        //                        <input type=""text"" class=""form-control"" placeholder=""Enter a cron expression * * * * *"" id=""{id}_cron"">
        //                        <span class=""input-group-btn "">
        //                        <button class=""btn btn-default btn-sm btn-warning js-management-input-commands"" type=""button"" input-id=""{id}""
        //                             data-url=""{Url.To(url)}"" data-loading-text=""{loadingText}"">
        //                            <span class=""glyphicon glyphicon-repeat""></span>
        //                            &nbsp;重复执行</button>
        //                        </span>
        //                    </div>
        //                </div>
        //               ";
        //}

        //private string Input(string id, string labelText, string placeholderText, string inputtype)
        //{
        //    return $@"
        //            <div class=""form-group"">
        //                <label for=""{id}"" class=""control-label"">{labelText}</label>
        //                <input class=""form-control"" type=""{inputtype}"" placeholder=""{placeholderText}"" id=""{id}"" >
        //            </div>
        //    ";
        //}

        //protected string InputTextbox(string id, string labelText, string placeholderText)
        //{
        //    return Input(id, labelText, placeholderText, "text").ToHtmlString();
        //}
        //protected string InputNumberbox(string id, string labelText, string placeholderText)
        //{
        //    return Input(id, labelText, placeholderText, "number").ToHtmlString();
        //}

        //protected string InputDatebox(string id, string labelText, string placeholderText)
        //{
        //    return $@"
        //            <div class=""form-group"">
        //                <label for=""{id}"" class=""control-label"">{labelText}</label>
        //                <div class='input-group date' id='{id}_datetimepicker'>
        //                    <input type='text' class=""form-control"" placeholder=""{placeholderText}"" />
        //                    <span class=""input-group-addon"">
        //                        <span class=""glyphicon glyphicon-calendar""></span>
        //                    </span>
        //                </div>
        //            </div>";
        //}

        //protected string InputCheckbox(string id, string labelText, string placeholderText)
        //{
        //    return $@"
        //                <div class=""form-group"">
        //                    <div class=""checkbox"">
        //                      <label>
        //                        <input type=""checkbox"" id=""{id}"">
        //                        {labelText}
        //                      </label>
        //                    </div>
        //                </div>
        //    ";
        //}
    }
}

namespace System.Web.WebPages
{
    internal class HelperResult //: IHtmlString
    {
        private readonly Action<IO.TextWriter> _action;

        /// <summary>This type/member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
        public HelperResult(Action<IO.TextWriter> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            this._action = action;
        }

        /// <summary>This type/member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
        public string ToHtmlString()
        {
            return this.ToString();
        }

        /// <summary>This type/member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
        public override string ToString()
        {
            string result;
            using (IO.StringWriter stringWriter = new IO.StringWriter(Globalization.CultureInfo.InvariantCulture))
            {
                this._action(stringWriter);
                result = stringWriter.ToString();
            }
            return result;
        }

        /// <summary>This type/member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
        public void WriteTo(IO.TextWriter writer)
        {
            this._action(writer);
        }
    }
}