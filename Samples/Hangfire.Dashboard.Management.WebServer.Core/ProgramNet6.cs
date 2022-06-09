#if NET6_0_OR_GREATER

using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.Service;
using Hangfire.Heartbeat;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-us");
//System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration["HangfireTask:nameOrConnectionString"]);
//.AddDbContextCheck<Data.ApplicationDbContext>()
;

builder.Services.AddSamples(builder.Configuration);

builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    var env = context.HostingEnvironment;
    builder
        .AddJsonFile("App_Data/appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"App_Data/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
})
.ConfigureLogging((hostingContext, logging) =>
{
    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventSourceLogger();
#if !NETFRAMEWORK
    logging.AddEventLog();
#endif
});









var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHealthChecks("/health");

app.UseSamples();

app.Run();

#endif
