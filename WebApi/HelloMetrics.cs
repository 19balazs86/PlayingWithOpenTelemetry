using System.Diagnostics.Metrics;

namespace WebApi;

public static class HelloMetrics
{
    public static Meter Meter = new Meter(Program.ServiceName);

    public static Counter<int> HelloCounter = Meter.CreateCounter<int>("hello.get");

    public static void IncrementCounter(string name, int selectedStatusCode)
    {
        HelloCounter.Add(1,
            new KeyValuePair<string, object?>("name", name),
            new KeyValuePair<string, object?>("status-code", selectedStatusCode));
    }
}
