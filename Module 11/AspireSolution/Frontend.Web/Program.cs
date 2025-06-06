using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Frontend.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddOpenTelemetry()
        .ConfigureResource(res => res.AddService("Web"))
         .UseAzureMonitor(conf => {
             conf.ConnectionString = @"InstrumentationKey=5bfd812c-b01d-4328-aadb-f8d8b0b6542c;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=7ba7fc8b-f165-464f-97d9-83f24bb81c72";
         })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            metrics.AddOtlpExporter();
        })
        .WithTracing(trace =>
        {
            trace
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation();
            trace.AddOtlpExporter();
        });
        builder.Logging.AddOpenTelemetry(log => log.AddOtlpExporter());

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient("weather", conf => {
            conf.BaseAddress = new Uri("https://localhost:7244");
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
