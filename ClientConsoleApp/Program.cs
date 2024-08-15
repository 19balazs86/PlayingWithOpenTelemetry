using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ClientConsoleApp;

public static class Program
{
    public static readonly ActivitySource ClientActivities = new ActivitySource("ClientActivities");

    public static async Task Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddHelloClient();

        services
            .AddOpenTelemetry()
            //.UseAzureMonitor() // Install-Package Azure.Monitor.OpenTelemetry.AspNetCore | Both can be applied: UseAzureMonitor and AddOtlpExporter
            .ConfigureResource(builder => builder.AddService("ClientApp", serviceInstanceId: Environment.MachineName))
            .WithTracing(configureTracing);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Obtaining the TracerProvider triggers the build and kicks off the tracing
        using TracerProvider tracerProvider = serviceProvider.GetRequiredService<TracerProvider>();

        //// This is more of a console application approach for obtaining the TracerProvider
        //using TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
        //    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PlayingWithOpenTelemetry"))
        //    .AddSource(_myActivitySource.Name)
        //    .AddJaegerExporter()
        //    .Build()!;

        IHelloClient helloClient = serviceProvider.GetRequiredService<IHelloClient>();

        using Activity activity = ClientActivities.StartActivity("Parallel-tasks")!;

        IEnumerable<Task> tasks = Enumerable.Range(1, 5).Select(n => helloClient.SayHello($"Balazs{n}"));

        await Task.WhenAll(tasks);
    }

    private static void configureTracing(TracerProviderBuilder tpBuilder)
    {
        tpBuilder.AddSource(ClientActivities.Name);

        tpBuilder.AddHttpClientInstrumentation(); // Outgoing HTTP call: Instrumentation.Http

        tpBuilder.AddOtlpExporter(); // Aspire-dashboard and jaeger-all-in-one has an OTLP receiver gRPC endpoint on port 4317

        tpBuilder.SetSampler<AlwaysOnSampler>(); // Only for DEV
    }
}
