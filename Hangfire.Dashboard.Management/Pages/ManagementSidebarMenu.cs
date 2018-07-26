using System;
using System.Collections.Generic;
using Hangfire.Dashboard;

namespace Hangfire.Dashboard.Management.Pages
{
    public static class ManagementSidebarMenu
    {
        public static List<Func<RazorPage, MenuItem>> Items = new List<Func<RazorPage, MenuItem>>();
    }
}