using System;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Annotations;

namespace Hangfire.Dashboard.Management.Support
{
    internal class EmbeddedResourceDispatcher : IDashboardDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;
        private readonly string _contentType;

        public EmbeddedResourceDispatcher(
            [NotNull] string contentType,
            [NotNull] Assembly assembly,
            string resourceName)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            _assembly = assembly;
            _resourceName = resourceName;
            _contentType = contentType;
        }

        public Task Dispatch(DashboardContext context)
        {
            //context.Response.ContentType = _contentType;
            //context.Response.SetExpire(DateTimeOffset.Now.AddYears(1));

            WriteResponse(context.Response);

            return Task.FromResult(true);
        }

        protected virtual void WriteResponse(DashboardResponse response)
        {
            WriteResource(response, _assembly, _resourceName);
        }

        protected void WriteResource(DashboardResponse response, Assembly assembly, string resourceName)
        {
            using (var inputStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (inputStream == null)
                {
                    throw new ArgumentException($@"Resource with name {resourceName} not found in assembly {assembly}.");
                }

                inputStream.CopyTo(response.Body);
            }
        }
    }

    internal class CombinedResourceDispatcher : EmbeddedResourceDispatcher
    {
        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly string[] _resourceNames;

        public CombinedResourceDispatcher(
            [NotNull] string contentType,
            [NotNull] Assembly assembly,
            string baseNamespace,
            params string[] resourceNames) : base(contentType, assembly, null)
        {
            _assembly = assembly;
            _baseNamespace = baseNamespace;
            _resourceNames = resourceNames;
        }

        protected override void WriteResponse(DashboardResponse response)
        {
            foreach (var resourceName in _resourceNames)
            {
                WriteResource(
                    response,
                    _assembly,
                    $"{_baseNamespace}.{resourceName}");
            }
        }
    }

    internal class CommandDispatcher : IDashboardDispatcher
    {
        private readonly Func<DashboardContext, bool> _command;

        public CommandDispatcher(Func<DashboardContext, bool> command)
        {
            _command = command;
        }

        public Task Dispatch(DashboardContext context)
        {
            var request = context.Request;
            var response = context.Response;

            //if (!"POST".Equals(request.Method, StringComparison.OrdinalIgnoreCase))
            //{
            //    response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
            //    return Task.FromResult(false);
            //}

            if (_command(context))
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.NoContent;
            }
            else
            {
                response.StatusCode = 422;
            }

            return Task.FromResult(true);
        }
    }

    //internal class DynamicJsDispatcher : IDashboardDispatcher
    //{
    //    public Task Dispatch(DashboardContext context)
    //    {
    //        var builder = new StringBuilder();


    //        return context.Response.WriteAsync(builder.ToString());
    //    }
    //}
}