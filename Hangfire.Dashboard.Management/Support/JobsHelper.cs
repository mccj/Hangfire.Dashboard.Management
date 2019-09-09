using Hangfire.Dashboard.Management.Metadata;

namespace Hangfire.Dashboard.Management.Support
{
    public static class JobsHelper
    {
        public static System.Collections.Generic.List<JobMetadata> Metadata { get; private set; }
        //internal static List<ManagementPageAttribute> Pages { get; set; } = new List<ManagementPageAttribute>();

        //internal static void GetAllJobs(params Assembly[] assembly)
        //{
        //    if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        //    GetAllJobs(assembly.SelectMany(f => f.GetTypes()));
        //}
        //internal static void GetAllJobs(IEnumerable<Type> types)
        //{
        //    if (types == null) throw new ArgumentNullException(nameof(types));

        //    var getQueue = new Func<string, string>(queue => string.IsNullOrWhiteSpace(queue) ? States.EnqueuedState.DefaultQueue : queue);
        //    GetAllJobs(() =>
        //    {
        //        return types
        //        //.Where(x => x.IsInterface && typeof(IJob).IsAssignableFrom(x) && x.Name != (typeof(IJob).Name))
        //        .Where(x => !x.IsInterface && typeof(IJob).IsAssignableFrom(x)/* && x.Name != (typeof(IJob).Name)*/)
        //        .Select(ti => new { Type = ti, ManagePage = ti.GetCustomAttribute<ManagementPageAttribute>() })
        //        .Where(ti => ti != null && ti.ManagePage != null)
        //        .Select(ti => new ManagePage(ti.ManagePage.Title, ti.ManagePage.MenuName, getQueue(ti.ManagePage.Queue), ti.Type.GetMethods().Select(tm => new JobMetadata
        //        {
        //            Type = tm.ReflectedType,
        //            Queue = getQueue(ti.ManagePage.Queue),
        //            MethodInfo = tm,
        //            Description = tm.GetCustomAttribute<DescriptionAttribute>()?.Description,
        //            DisplayName = tm.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
        //        })));
        //    });


        //    //Metadata = new List<JobMetadata>();
        //    //Pages = new List<ManagementPageAttribute>();

        //    //foreach (var ti in types.Where(x => x.IsInterface && typeof(IJob).IsAssignableFrom(x) && x.Name != (typeof(IJob).Name)))
        //    //{
        //    //    var queue = States.EnqueuedState.DefaultQueue;
        //    //    if (ti.GetCustomAttributes(true).OfType<ManagementPageAttribute>().Any())
        //    //    {
        //    //        var attr = ti.GetCustomAttribute<ManagementPageAttribute>();
        //    //        queue = attr.Queue;
        //    //        Pages.Add(attr);
        //    //    }

        //    //    foreach (var methodInfo in ti.GetMethods())
        //    //    {
        //    //        var meta = new JobMetadata { Type = ti, Queue = queue };
        //    //        if (methodInfo.GetCustomAttributes<DescriptionAttribute>(true).OfType<DescriptionAttribute>().Any())
        //    //        {
        //    //            meta.Description = methodInfo.GetCustomAttribute<DescriptionAttribute>().Description;
        //    //        }

        //    //        if (methodInfo.GetCustomAttributes(true).OfType<DisplayNameAttribute>().Any())
        //    //        {
        //    //            meta.MethodInfo = methodInfo;
        //    //            meta.DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
        //    //        }

        //    //        Metadata.Add(meta);
        //    //    }
        //    //}
        //}
        //internal static void GetAllJobs(Func<IEnumerable<ManagePage>> typesProvider)
        //{
        //    if (typesProvider == null) throw new ArgumentNullException(nameof(typesProvider));

        //    Metadata = new List<JobMetadata>();
        //    Pages = new List<ManagementPageAttribute>();

        //    foreach (var ti in typesProvider().Where(f => f.Metadatas != null && f.Metadatas.Any()))
        //    {
        //        Pages.Add(new ManagementPageAttribute(ti.Title, ti.MenuName, ti.Queue));
        //        var queue = string.IsNullOrWhiteSpace(ti.Queue) ? States.EnqueuedState.DefaultQueue : ti.Queue;

        //        foreach (var item in ti.Metadatas)
        //        {
        //            Metadata.Add(item);
        //        }
        //    }
        //}
    }
    public class ManagePage
    {
        public ManagePage(string title, /*string menuName, string queue, */JobMetadata[] metadatas)
        {
            Title = title;
            //MenuName = menuName;
            //Queue = queue;
            Metadatas = metadatas;
        }
        public string Title { get; }
        //public string MenuName { get; }
        //public string Queue { get; }
        public JobMetadata[] Metadatas { get; }
    }
}
