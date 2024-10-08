using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebApi.Miscellaneous;

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

            // loggingBuilder.AddOpenTelemetry(configureLogging); // This works as well

            services
                .AddOpenTelemetry()
                .ConfigureResource(configureResource) // Globally set for Tracing, Metrics and Logs
                //.UseAzureMonitor() // Install-Package Azure.Monitor.OpenTelemetry.AspNetCore | Both can be applied: UseAzureMonitor and UseOtlpExporter
                .UseOtlpExporter() // Globally set for Tracing, Metrics and Logs | The endpoint is set to localhost by default, but you can configure it with the environment variables OTEL_EXPORTER_OTLP_ENDPOINT and OTEL_EXPORTER_OTLP_PROTOCOL
                .WithTracing(configureTracing)
                .WithMetrics(configureMetrics)
                .WithLogging(loggerProviderBuilder => { }, configureLogging);
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

        // https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/extending-the-sdk/MyFilteringProcessor.cs
        // builder.AddProcessor<FilteringProcessor>();
    }

    private static void configureMetrics(MeterProviderBuilder builder)
    {
        builder.AddAspNetCoreInstrumentation();

        // Read more: https://github.com/open-telemetry/opentelemetry-dotnet-contrib/tree/main/src/OpenTelemetry.Instrumentation.Runtime
        // builder.AddRuntimeInstrumentation(); // Additional metrics

        builder.AddMeter(DiagnosticConfig.Meter.Name);
    }

    private static void configureLogging(OpenTelemetryLoggerOptions options)
    {
        // options.AddOtlpExporter(); // You can apply OTLP exporter for Tracing, Metrics and Logs globally with the UseOtlpExporter method

        options.AddProcessor(new ActivityEventLogProcessor());

        options.IncludeScopes           = true;
        options.IncludeFormattedMessage = true;
    }
}
