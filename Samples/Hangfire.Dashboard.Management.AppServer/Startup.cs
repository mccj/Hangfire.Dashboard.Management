//using Hangfire.Console;
using Hangfire.MemoryStorage;
using Owin;
using System;
using System.Linq;

namespace Hangfire.Dashboard.Management.Test
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Microsoft.Owin.IOwinContext owinContext;
            //owinContext.Request.ReadFormAsync()

            GlobalConfiguration.Configuration
            .UseColouredConsoleLogProvider()
            //.UseConsole()
            .UseManagementPages((cc) => cc.AddJobs(GetModuleTypes()))
            .UseMemoryStorage();

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }

        public static Type[] GetModuleTypes()
        {
            //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //var moduleDirectory = System.IO.Path.Combine(baseDirectory, "Modules");
            //var assembliePaths = System.IO.Directory.GetFiles(baseDirectory, "*.dll");
            //if (System.IO.Directory.Exists(moduleDirectory))
            //    assembliePaths = assembliePaths.Concat(System.IO.Directory.GetFiles(moduleDirectory, "*.dll")).ToArray();

            //var assemblies = assembliePaths.Select(f => System.Reflection.Assembly.LoadFile(f)).ToArray();
            var assemblies = new[] { typeof(HangfireJobTask.常规任务).Assembly };
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
    }
}