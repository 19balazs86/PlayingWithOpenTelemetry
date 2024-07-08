using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder  = WebApplication.CreateBuilder(args);
        IServiceCollection services    = builder.Services;
        ILoggingBuilder loggingBuilder = builder.Logging;

        // Add services to the container
        {
            services.AddControllers();

            loggingBuilder.AddOpenTelemetry(configureLogging);

            services
                .AddOpenTelemetry()
                .ConfigureResource(configureResource) // Globally set for Tracing, Metrics and Logs
                .UseOtlpExporter() // Globally set for Tracing, Metrics and Logs | Aspire-dashboard and jaeger-all-in-one has an OTLP receiver gRPC endpoint on port 4317
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

    private static void configureResource(ResourceBuilder resBuilder)
    {
        resBuilder.AddService(
            serviceName: DiagnosticConfig.ServiceName,
            serviceVersion: DiagnosticConfig.ServiceVersion,
            serviceInstanceId: Environment.MachineName);

        // resBuilder.AddAttributes([KeyValuePair.Create<string, object>("MyKey", "MyValue")]);
    }

    private static void configureTracing(TracerProviderBuilder builder)
    {
        // Incoming HTTP request: Instrumentation.AspNetCore
        builder.AddAspNetCoreInstrumentation();

        // builder.AddOtlpExporter(); // You can apply OTLP exporter for Tracing, Metrics and Logs globally with the UseOtlpExporter method

        builder.SetSampler<AlwaysOnSampler>(); // Only for DEV
    }

    private static void configureMetrics(MeterProviderBuilder builder)
    {
        builder.AddAspNetCoreInstrumentation();

        builder.AddMeter(DiagnosticConfig.Meter.Name);
    }

    private static void configureLogging(OpenTelemetryLoggerOptions options)
    {
        // options.AddOtlpExporter(); // You can apply OTLP exporter for Tracing, Metrics and Logs globally with the UseOtlpExporter method

        options.IncludeScopes           = true;
        options.IncludeFormattedMessage = true;
    }
}
