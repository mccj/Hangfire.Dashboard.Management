//using Hangfire.Console;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Owin;
using System;
using System.ComponentModel;
using System.Linq;

namespace Hangfire.Dashboard.Management.Test
{
    [Hangfire.Dashboard.Management.Metadata.ManagementPage("测试", "test")]

    public class eBay商品收集
    {
        [Hangfire.Dashboard.Management.Support.Job]
        [DisplayName("eBayAnalysis_Item(产品)_采集_服务_执行数据采集")]
        [Description("使用eBay平台Api采集ItemId信息")]
        [AutomaticRetry(Attempts = 3)]//自动重试
        [DisableConcurrentExecution(90)]//禁用并行执行
        public void 商品收集RabbitMQ(PerformContext context = null)
        {
        }
    }
}
