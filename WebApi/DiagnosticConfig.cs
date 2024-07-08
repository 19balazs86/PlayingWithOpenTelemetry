using System.Diagnostics.Metrics;

namespace WebApi;

public static class DiagnosticConfig
{
    public const string ServiceName = "WebApi";

    public static string ServiceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

    //public static ActivitySource ActivitySource = new ActivitySource(ServiceName, ServiceVersion);

    public static Meter Meter = new Meter(ServiceName, ServiceVersion);

    public static Counter<int> HelloCounter = Meter.CreateCounter<int>("hello.get");

    public static void IncrementCounter(string name, int selectedStatusCode)
    {
        HelloCounter.Add(1,
            new KeyValuePair<string, object?>("name", name),
            new KeyValuePair<string, object?>("status-code", selectedStatusCode));
    }
}
