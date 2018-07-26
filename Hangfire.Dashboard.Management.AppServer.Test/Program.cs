//using Hangfire.Console;
//using Hangfire.SqlServer.RabbitMQ;
using Microsoft.Owin.Hosting;
using Topshelf;

namespace Hangfire.Dashboard.Management.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = HostFactory.Run(x =>
             {
                 x.Service<WebAppService>(sc =>
                 {
                     sc.ConstructUsing(name => new WebAppService());

                     //sc.WhenStarted((s, hostControl) =>
                     //{
                     //    s.HostPort = 3000;
                     //    return s.Start(hostControl);
                     //});//开始启动
                     sc.WhenStarted((s, hostControl) => s.Start(hostControl));//启动
                     sc.WhenStopped((s, hostControl) => s.Stop(hostControl));//停止
                                                                             //                                                        // optional pause/continue methods if used
                                                                             //sc.WhenPaused(s => s.Pause());
                                                                             //sc.WhenContinued(s => s.Continue());

                     // optional, when shutdown is supported
                     //sc.WhenShutdown(s => s.Shutdown());//关闭
                 });
                 x.RunAsLocalSystem();
                 x.OnException(ex =>
                 {

                     System.Console.WriteLine("Exception thrown - " + ex.Message);
                     //使用异常的东西
                 });
             });
        }
    }
    class WebAppService : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            System.Console.WriteLine("http://localhost:1101/hangfire");
            var options = new StartOptions()
            {
                Port = 1101
            };
            WebApp.Start<Startup>(options);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}
