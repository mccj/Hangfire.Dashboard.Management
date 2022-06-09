#if !NET6_0_OR_GREATER
using Hangfire.Annotations;
using Hangfire.Console;
using Hangfire.Heartbeat;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETCOREAPP3_1_OR_GREATER
using Microsoft.Extensions.Hosting;
#endif

namespace Hangfire.Dashboard.Management.Service
{
    public class Startup
    {
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


#if NETCOREAPP2_2 || NETFRAMEWORK
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
#else
            services.AddControllersWithViews();
#endif

            services.AddHealthChecks()
#if !NETFRAMEWORK
            .AddSqlServer(Configuration["HangfireTask:nameOrConnectionString"]);
            //.AddDbContextCheck<Data.ApplicationDbContext>()
#endif
            ;

            //System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-us");
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            services.AddSamples(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
#if NETCOREAPP2_2 || NETFRAMEWORK
            IHostingEnvironment env
#else
            IWebHostEnvironment env
#endif
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
#if NETCOREAPP2_2 || NETFRAMEWORK
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
#else
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
#endif
            app.UseHealthChecks("/health");

            app.UseSamples();
        }
    }
}
#endif