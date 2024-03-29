﻿using Hangfire;
using Hangfire.Annotations;
using Hangfire.Console;
using Hangfire.Dashboard.Management;
using Hangfire.Dashboard.Management.Service;
using Hangfire.Heartbeat;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.LiteDB;
using Hangfire.Storage.SQLite;
using Hangfire.MySql;

public class Samples
{ }

public static class SamplesExtensions
{
    public static IServiceCollection AddSamples(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HangfireServiceOption>(configuration.GetSection("HangfireTask"));
        services.AddHangfire((sp, x) =>
        {
            var logger = sp.GetService<ILogger<Samples>>();
            try
            {
                var _hangfireOption = sp.GetService<Microsoft.Extensions.Options.IOptions<HangfireServiceOption>>()?.Value;
                var queues = _hangfireOption?.Queues?.ToLower()?.Replace("-", "_")?.Replace(" ", "_")?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();//new[] { "default", "apis", "jobs" };
                if (queues == null || queues.Length == 0) queues = new[] { Hangfire.States.EnqueuedState.DefaultQueue };

                x
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_110)//设置数据兼容级别
                    .UseSimpleAssemblyNameTypeSerializer()//使用简单程序集名称类型序列化程序
                    .UseRecommendedSerializerSettings()//使用推荐的序列化程序设置
                    .UseColouredConsoleLogProvider()//使用彩色控制台日志提供程序
                    .UseConsole()//使用控制台程序(Hangfire.Console)
                                 //.UseLog4NetLogProvider()//使用log4net日志提供程序
                                 //.UseNLogLogProvider()//使用NLogLog日志提供程序

                    //.UseActivator(new OrchardJobActivator(_lifetimeScope))
                    //.UseFilter(new LogFailureAttribute())//登录失败日志记录
                    .UseHeartbeatPage(checkInterval: TimeSpan.FromSeconds(1))
                    .UseManagementPages((config) =>
                    {
                        return config
                            .AddJobs(GetModuleTypes())
                            //.SetCulture(cultureInfo)
                            //.TranslateJson(< Custom language JSON >)
                            ////or
                            //.TranslateCulture(< Custom Language Object >)
                            ////or
                            //.TranslateStream(< Custom language Stream >);
                            ;
                    })

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

                switch (_hangfireOption.StorageType)
                {
                    case StorageType.SqlServerStorage:
                        UseSqlServerStorage(x, _hangfireOption.nameOrConnectionString, queues);
                        break;

                    case StorageType.MemoryStorage:
                        x.UseMemoryStorage();
                        break;
                    //case Settings.StorageType.FirebirdStorage:
                    //    break;
                    //case Settings.StorageType.RedisStorage:
                    //    //config.UseRedisStorage();
                    //    break;
                    //case Settings.StorageType.FirebaseStorage:
                    //    break;
                    //case Settings.StorageType.MongoStorage:
                    //    break;
                    case StorageType.MySqlStorage:
                        x.UseStorage(new MySqlStorage(
                             _hangfireOption.nameOrConnectionString,
                            new MySqlStorageOptions
                            {
                                //TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                                //QueuePollInterval = TimeSpan.FromSeconds(15),
                                //JobExpirationCheckInterval = TimeSpan.FromHours(1),
                                //CountersAggregateInterval = TimeSpan.FromMinutes(5),
                                //PrepareSchemaIfNecessary = true,
                                //DashboardJobListLimit = 50000,
                                //TransactionTimeout = TimeSpan.FromMinutes(1),
                                TablesPrefix = "Hangfire"
                            }));
                        break;
                    //case Settings.StorageType.PostgreSqlStorage:
                    //    break;
                    //case Settings.StorageType.RavenStorage:
                    //    break;
                    case StorageType.LiteDbStorage:
                        x.UseLiteDbStorage();
                        break;
                    case StorageType.SQLiteStorage:
                        x.UseSQLiteStorage();
                        break;
                    case StorageType.LocalStorage:
                    default:
                        {
                            UseSqlServerStorage(x, $"Data Source=(LocalDb)\\MSSQLLocalDB;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\\Hangfire.mdf", queues);
                        }
                        break;
                }
                //.UseSqlServerStorage("server=10.11.1.121;database=Hangfire;uid=sa;pwd=`1q2w3e4r;Application Name=WebErpApp (Hangfire) Data Provider")
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "AddHangfire 异常");
                throw;
            }
        });
        services.AddHangfireServerEx((sp, options) =>
        {
            var _hangfireOption = sp.GetService<Microsoft.Extensions.Options.IOptions<HangfireServiceOption>>()?.Value;
            //var _hangfireOption = _hangfireServiceOption?.Value;

            if (_hangfireOption?.IsUseHangfireServer == true)
            {
                var queues = _hangfireOption.Queues?.ToLower()/*?.Replace("-", "_")*/?.Replace(" ", "_")?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();//new[] { "default", "apis", "jobs" };
                if (queues == null || queues.Length == 0) queues = new[] { Hangfire.States.EnqueuedState.DefaultQueue };
                //启用本地服务
                options = new BackgroundJobServerOptions
                {//:{System.Web.Hosting.HostingEnvironment.SiteName }:{System.Web.Hosting.HostingEnvironment.ApplicationID}
                    ServerName = $"{(string.IsNullOrWhiteSpace(_hangfireOption?.ServiceName) ? "" : ("[" + _hangfireOption?.ServiceName + "]"))}{Environment.MachineName}:{System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}:{AppDomain.CurrentDomain.FriendlyName}:{System.Diagnostics.Process.GetCurrentProcess().Id}:{AppDomain.CurrentDomain.Id}",
                    //[]mccj-pc:webuiapp:32732:1
                    ShutdownTimeout = TimeSpan.FromMinutes(30),//关闭超时时间
                    WorkerCount = Math.Max(Environment.ProcessorCount, _hangfireOption.WorkerCount == 0 ? 20 : _hangfireOption.WorkerCount),//最大job并发处理数量
                    Queues = queues
                };

                return true;
            }
            return false;
        });
        return services;
    }

    public static void UseSamples(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetService<ILogger<Samples>>();
        try
        {
            var serviceProvider = app.ApplicationServices;
            var _hangfireOption = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<HangfireServiceOption>>()?.Value;
            ////var _hangfireOption = _hangfireServiceOption?.Value;

            //if (_hangfireOption?.IsUseHangfireServer == true)
            //{
            //    var queues = _hangfireOption.Queues?.ToLower()?.Replace("-", "_")?.Replace(" ", "_")?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();//new[] { "default", "apis", "jobs" };
            //    if (queues == null || queues.Length == 0) queues = new[] { Hangfire.States.EnqueuedState.DefaultQueue };
            //    //启用本地服务
            //    var options = new BackgroundJobServerOptions
            //    {//:{System.Web.Hosting.HostingEnvironment.SiteName }:{System.Web.Hosting.HostingEnvironment.ApplicationID}
            //        ServerName = $"{(string.IsNullOrWhiteSpace(_hangfireOption?.ServiceName) ? "" : ("[" + _hangfireOption?.ServiceName + "]"))}{Environment.MachineName}:{System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}:{AppDomain.CurrentDomain.FriendlyName}:{System.Diagnostics.Process.GetCurrentProcess().Id}:{AppDomain.CurrentDomain.Id}",
            //        //[]mccj-pc:webuiapp:32732:1
            //        ShutdownTimeout = TimeSpan.FromMinutes(30),//关闭超时时间
            //        WorkerCount = Math.Max(Environment.ProcessorCount, _hangfireOption.WorkerCount == 0 ? 20 : _hangfireOption.WorkerCount),//最大job并发处理数量
            //        Queues = queues
            //    };
            //    _logger.LogInformation("启动服务");
            //    app.UseHangfireServer(options, new[] { new Heartbeat.Server.ProcessMonitor(checkInterval: TimeSpan.FromSeconds(1)) });
            //}
            if (_hangfireOption?.IsUseHangfireDashboard == true)
            {
                logger.LogInformation("启动面板");
                ////启用面板
                app.UseHangfireDashboard(_hangfireOption.HangfireDashboardUrl,
                    new DashboardOptions
                    {
                        //默认授权远程无法访问 Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter
                        Authorization = new[] { new DashboardAuthorizationFilter() },//授权

                        //AppPath = System.Web.VirtualPathUtility.ToAbsolute("~/"),//返回站点链接URL
                        //DisplayStorageConnectionString = false,
                        //IsReadOnlyFunc = f => true,
                        //DashboardTitle = "ASP.Net Core Hangfire Management",
                        //StatsPollingInterval = 5000
                    }
                    );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hangfire 异常");
            throw;
        }
        //app.UseHangfireServer();//启动Hangfire服务
        //app.UseHangfireDashboard();//启动hangfire面板
    }

    public static Type[] GetModuleTypes()
    {
        //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var moduleDirectory = System.IO.Path.Combine(baseDirectory, "Modules");
        //var assembliePaths = System.IO.Directory.GetFiles(baseDirectory, "*.dll");
        //if (System.IO.Directory.Exists(moduleDirectory))
        //    assembliePaths = assembliePaths.Concat(System.IO.Directory.GetFiles(moduleDirectory, "*.dll")).ToArray();

        //var assemblies = assembliePaths.Select(f => System.Reflection.Assembly.LoadFile(f)).ToArray();
        //var assemblies = new[] { typeof(HangfireJobTask.常规任务).Assembly };

        //var moduleTypes = assemblies.SelectMany(f =>
        //{
        //    try
        //    {
        //        return f.GetTypes();
        //    }
        //    catch (Exception)
        //    {
        //        return new Type[] { };
        //    }
        //}

        //).Where(f => f.IsClass && !f.IsAbstract && !f.IsInterface)
        //.Where(f => f.GetCustomAttributes(true).Any(ff => ff is Hangfire.Dashboard.Management.Support.JobAttribute))
        //.ToArray();
        var moduleTypes = GetApplicationTypes();

        return moduleTypes;
    }

#if NETCOREAPP2_2
        private static Type[] GetApplicationTypes()
        {
            // Get all assembly and types.
            var deps = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
            var typeList = new System.Collections.Generic.List<Type>();
            deps.CompileLibraries
                // Ignore all system assembly and nuget pakage
                .Where(lib => !lib.Serviceable && lib.Type != "package")
                .ToList().ForEach(lib =>
                {
                    try
                    {
                        var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(new System.Reflection.AssemblyName(lib.Name));
                        typeList.AddRange(assembly.GetTypes().Where(type => type != null));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Load deps error.", e);
                    }
                });
            return typeList.ToArray();
        }
#else

    private static Type[] GetApplicationTypes()
    {
        //var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load);
        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assemblies = new[] { typeof(HangfireJobTask.常规任务).Assembly };
        var types = assemblies.SelectMany(f => f.GetTypes()).ToArray();
        return types;
    }

#endif

    private static void UseSqlServerStorage(IGlobalConfiguration config, string nameOrConnectionString, string[] queues)
    {
        var configSql = config
            .UseDashboardMetric(Hangfire.SqlServer.SqlServerStorage.ActiveConnections)//活动连接数量
            .UseDashboardMetric(Hangfire.SqlServer.SqlServerStorage.TotalConnections)//总连接数量
            .UseSqlServerStorage(nameOrConnectionString, new Hangfire.SqlServer.SqlServerStorageOptions
            {
                //QueuePollInterval = TimeSpan.FromSeconds(1),
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true//,
                                         //EnableHeavyMigrations = true
            })
            ;
        try
        {
            //var msmq = System.Messaging.MessageQueue.GetMachineId(Environment.MachineName);
            //configSql.UseMsmqQueues(@".\Private$\hangfire{0}", queues);
        }
        catch (Exception /*ex*/)
        {
            //try
            //{
            //    configSql.Entry.UseRabbitMq(f =>
            //    {
            //        f.Username = "";
            //        f.Password = "";
            //        f.HostName = "";
            //        f.VirtualHost = "";
            //        //f.Port = 0;
            //        //f.Uri = new Uri("");
            //    }, queues);
            //}
            //catch (Exception ex2) { }
        }
    }
}

public static class MyHangfireServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireServerEx([Hangfire.Annotations.NotNull] this IServiceCollection services, Func<IServiceProvider, BackgroundJobServerOptions, bool> optionsFun)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (optionsFun == null) throw new ArgumentNullException(nameof(optionsFun));

        return AddHangfireServerInner(services, null, null, optionsFun);
    }

    private static IServiceCollection AddHangfireServerInner([NotNull] IServiceCollection services, [CanBeNull] JobStorage storage, [CanBeNull] IEnumerable<IBackgroundProcess> additionalProcesses, [NotNull] Func<IServiceProvider, BackgroundJobServerOptions, bool> optionsFun)
    {
        services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, BackgroundJobServerHostedService>(provider =>
        {
            var backgroundJobServerOptions = provider.GetService<BackgroundJobServerOptions>() ?? new BackgroundJobServerOptions();
            if (optionsFun(provider, backgroundJobServerOptions))
                return CreateBackgroundJobServerHostedService(provider, storage, additionalProcesses, backgroundJobServerOptions);
            else
                return null;
        });
        return services;
    }

    private static BackgroundJobServerHostedService CreateBackgroundJobServerHostedService(IServiceProvider provider, JobStorage storage, IEnumerable<IBackgroundProcess> additionalProcesses, BackgroundJobServerOptions options)
    {
        var m = typeof(HangfireServiceCollectionExtensions).GetMethod("CreateBackgroundJobServerHostedService", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        return m.Invoke(null, new object[] { provider, storage, additionalProcesses, options }) as BackgroundJobServerHostedService;
    }
}