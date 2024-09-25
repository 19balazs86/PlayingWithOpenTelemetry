using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace WebApi;

// This processor will attach the log record as an event to the activity, allowing you to view it in the Aspire Dashboard
// If you are using Azure Monitor, this processor is unnecessary, as the related logs can be seen in App Insights
public sealed class ActivityEventLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        base.OnEnd(data);

        Activity? currentActivity = Activity.Current;

        currentActivity?.AddEvent(new ActivityEvent(data.Attributes?.ToString() ?? string.Empty));
    }
}
