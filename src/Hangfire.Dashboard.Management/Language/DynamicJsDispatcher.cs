//using Hangfire.Dashboard;
//using System;
//using System.Text;
//using System.Threading.Tasks;

//namespace Hangfire.Dashboard.Management
//{
//    /// <summary>
//    /// 翻译
//    /// </summary>
//    internal class DynamicJsDispatcher : IDashboardDispatcher
//    {
//        public Task Dispatch(DashboardContext context)
//        {
//            var builder = new StringBuilder();
//            //builder.Append("moment.defineLocale('zh', { parentLocale: 'zh-cn' });")
//            builder
//                .AppendLine(@"
//        var zhcnlocale = moment.localeData('zh-cn');
//        moment.locale('zh', {
//            months: zhcnlocale._months,
//            monthsShort: zhcnlocale._monthsShort,
//            weekdays: zhcnlocale._weekdays,
//            weekdaysShort: zhcnlocale._weekdaysShort,
//            weekdaysMin: zhcnlocale._weekdaysMin,
//            longDateFormat: zhcnlocale._longDateFormat,
//            meridiemParse: zhcnlocale._meridiemParse,
//            meridiemHour: zhcnlocale.meridiemHour,
//            meridiem: zhcnlocale.meridiem,
//            calendar: zhcnlocale._calendar,
//            dayOfMonthOrdinalParse: zhcnlocale._dayOfMonthOrdinalParse,
//            ordinal: zhcnlocale.ordinal,
//            relativeTime: zhcnlocale._relativeTime,
//            week: zhcnlocale._week
//        });")
//                   //
//                   //builder.Append(@"(function (hangFire) {")
//                   //       .Append("hangFire.config = hangFire.config || {};")
//                   //.AppendFormat("hangFire.config.consolePollInterval = {0};", _options.PollInterval)
//                   //.AppendFormat("hangFire.config.consolePollUrl = '{0}/console/';", context.Request.PathBase)
//                   //.Append("})(window.Hangfire = window.Hangfire || {});")
//                   .AppendLine();

//            return context.Response.WriteAsync(builder.ToString());
//        }
//    }
//}