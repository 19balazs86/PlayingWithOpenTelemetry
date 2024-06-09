using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi;

public static class Program
{
    private const string _serviceName = "WebApi";

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder  = WebApplication.CreateBuilder(args);
        IServiceCollection services    = builder.Services;
        ILoggingBuilder loggingBuilder = builder.Logging;

        // Add services to the container
        {
            services.AddControllers();

            //loggingBuilder.AddOpenTelemetry(options => options
            //    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_serviceName))
            //    .AddOtlpExporter());

            services
                .AddOpenTelemetry()
                .ConfigureResource(resBuilder => resBuilder.AddService(_serviceName))
                .UseOtlpExporter() // Globally set OTLP exporter | Aspire-dashboard and jaeger-all-in-one has an OTLP receiver gRPC endpoint on port 4317
                .WithTracing(configureTracing)
                .WithMetrics(configureMetrics);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseCustomExceptionHandlingMiddleware();

            app.MapControllers();
        }

        app.Run();
    }

    private static void configureTracing(TracerProviderBuilder tpBuilder)
    {
        // Incoming HTTP request: Instrumentation.AspNetCore
        tpBuilder.AddAspNetCoreInstrumentation();

        // tpBuilder.AddOtlpExporter(); // You can apply OTLP globally for tracing and metrics with the UseOtlpExporter method

        tpBuilder.SetSampler<AlwaysOnSampler>(); // Only for DEV
    }

    private static void configureMetrics(MeterProviderBuilder mpBuilder)
    {

    }
}
