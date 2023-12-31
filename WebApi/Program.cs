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
                .ConfigureResource(builder => builder.AddService(_serviceName))
                .WithTracing(builder => builder
                    .AddAspNetCoreInstrumentation() // Incoming HTTP request: Instrumentation.AspNetCore
                    .AddOtlpExporter()); // jaeger-all-in-one has an Otlp receiver gRPC endpoint on port 4317
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseExceptionHandlingMiddleware();

            app.MapControllers();
        }

        app.Run();
    }
}
