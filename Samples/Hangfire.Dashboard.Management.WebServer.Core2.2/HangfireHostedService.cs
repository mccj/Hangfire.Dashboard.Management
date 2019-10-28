//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Hangfire.Dashboard.Management.Service
//{
//    public class HangfireHostedService : IHostedService
//    {
//        private readonly Microsoft.AspNetCore.Builder.IApplicationBuilder _app;
//        private readonly ILogger _logger;
//        private HangfireHostedService(Microsoft.AspNetCore.Builder.IApplicationBuilder app, ILogger<HangfireHostedService> logger)
//        {
//            _app = app;
//            _logger = logger;
//        }
//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            //try
//            //{
//            //    var serviceProvider = _app.ApplicationServices;
//            //    var _hangfireOption = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<HangfireServiceOption>>()?.Value;
//            //    //var _hangfireOption = _hangfireServiceOption?.Value;
//            //    var queues = _hangfireOption.Queues?.ToLower()?.Replace("-", "_")?.Replace(" ", "_")?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();//new[] { "default", "apis", "jobs" };
//            //    if (queues == null || queues.Length == 0) queues = new[] { Hangfire.States.EnqueuedState.DefaultQueue };

//            //    if (_hangfireOption?.IsUseHangfireServer == true)
//            //    {
//            //        //启用本地服务
//            //        //var options = new BackgroundJobServerOptions
//            //        //{//:{System.Web.Hosting.HostingEnvironment.SiteName }:{System.Web.Hosting.HostingEnvironment.ApplicationID}
//            //        //    ServerName = $"{(string.IsNullOrWhiteSpace(_hangfireOption?.ServiceName) ? "" : ("[" + _hangfireOption?.ServiceName + "]"))}{Environment.MachineName}:{System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}:{AppDomain.CurrentDomain.FriendlyName}:{System.Diagnostics.Process.GetCurrentProcess().Id}:{AppDomain.CurrentDomain.Id}",
//            //        //    //[]mccj-pc:webuiapp:32732:1
//            //        //    ShutdownTimeout = TimeSpan.FromMinutes(30),//关闭超时时间
//            //        //    WorkerCount = Math.Max(Environment.ProcessorCount, _hangfireOption.WorkerCount == 0 ? 20 : _hangfireOption.WorkerCount),//最大job并发处理数量
//            //        //    Queues = queues
//            //        //};
//            //        _logger.LogInformation("启动服务");
//            //        _app.UseHangfireServer(/*options*/);
//            //    }
//            //    if (_hangfireOption?.IsUseHangfireDashboard == true)
//            //    {
//            //        _logger.LogInformation("启动面板");
//            //        ////启用面板
//            //        _app.UseHangfireDashboard(_hangfireOption.HangfireDashboardUrl,
//            //            new DashboardOptions
//            //            {
//            //                //默认授权远程无法访问 Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter 
//            //                Authorization = new[] { new DashboardAuthorizationFilter() },//授权
//            //                                                                             //AppPath = System.Web.VirtualPathUtility.ToAbsolute("~/"),//返回站点链接URL
//            //            }
//            //            );
//            //    }
//            //}
//            //catch (Exception ex)
//            //{
//            //    _logger.LogError(ex, "Hangfire 异常");
//            //    throw;
//            //}

//            //throw new System.NotImplementedException();
//            return Task.CompletedTask;
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            //throw new System.NotImplementedException();
//            return Task.CompletedTask;
//        }
//    }
//}
