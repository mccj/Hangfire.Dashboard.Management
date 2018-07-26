using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.Dashboard.Management;

namespace Hangfire.Dashboard.Management.Standard.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddHangfire(x =>
            {
                x
                    .UseColouredConsoleLogProvider()//使用彩色控制台日志提供程序
                    //.UseLog4NetLogProvider()//使用log4net日志提供程序
                    //.UseNLogLogProvider()//使用NLogLog日志提供程序
                    //.UseConsole()//使用控制台程序(Hangfire.Console)

                    //.UseActivator(new OrchardJobActivator(_lifetimeScope))
                    //.UseFilter(new LogFailureAttribute())//登录失败日志记录

                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ServerCount)//服务器数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RecurringJobCount)//任务数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.RetriesCount)//重试次数
                    //.UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.EnqueuedCountOrNull)//队列数量
                    //.UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.FailedCountOrNull)//失败数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.EnqueuedAndQueueCount)//队列数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ScheduledCount)//计划任务数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.ProcessingCount)//执行中的任务数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.SucceededCount)//成功作业数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.FailedCount)//失败数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.DeletedCount)//删除数量
                    .UseDashboardMetric(Hangfire.Dashboard.DashboardMetrics.AwaitingCount)//等待任务数量
                ;
                //x
                //    .UseDashboardMetric(Hangfire.SqlServer.SqlServerStorage.ActiveConnections)//活动连接数量
                //    .UseDashboardMetric(Hangfire.SqlServer.SqlServerStorage.TotalConnections)//总连接数量
                //    .UseSqlServerStorage("server=10.11.1.11;database=Hangfire;uid=sa;pwd=`1q2w3e4r;Application Name=WebErpApp (Hangfire) Data Provider", new Hangfire.SqlServer.SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) })
                //;
                x.UseManagementPages((cc) => cc.AddJobs(GetModuleTypes()))
                    .UseMemoryStorage()
                    ;
            });
        }
        public static Type[] GetModuleTypes()
        {
            //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //var moduleDirectory = System.IO.Path.Combine(baseDirectory, "Modules");
            //var assembliePaths = System.IO.Directory.GetFiles(baseDirectory, "*.dll");
            //if (System.IO.Directory.Exists(moduleDirectory))
            //    assembliePaths = assembliePaths.Concat(System.IO.Directory.GetFiles(moduleDirectory, "*.dll")).ToArray();

            //var assemblies = assembliePaths.Select(f => System.Reflection.Assembly.LoadFile(f)).ToArray();
            var assemblies = new[] { typeof(Startup).Assembly };
            var moduleTypes = assemblies.SelectMany(f =>
            {
                try
                {
                    return f.GetTypes();
                }
                catch (Exception)
                {

                    return new Type[] { };
                }
            }


            )/*.Where(f => f.IsClass && typeof(IClientModule).IsAssignableFrom(f))*/.ToArray();

            return moduleTypes;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();

            app.UseHangfireServer();//启动Hangfire服务
            app.UseHangfireDashboard();//启动hangfire面板
        }
    }
}
