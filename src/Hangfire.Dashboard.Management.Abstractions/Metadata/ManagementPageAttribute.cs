using System;

namespace Hangfire.Dashboard.Management.Metadata
{
    public class ManagementPageAttribute : Attribute
    {
        public string Title { get; }

        //public string MenuName { get; }
        public string Queue { get; }

        public ManagementPageAttribute(string title/*, string menuName*/, string queue = null)
        {
            Title = title;
            //MenuName = menuName;
            Queue = queue;
        }
    }
}