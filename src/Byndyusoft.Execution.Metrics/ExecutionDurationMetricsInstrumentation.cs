using System.Diagnostics;

namespace Byndyusoft.Execution.Metrics;

/// <summary>
///     Инструмент, который слушает Activity запущенные из ExecutionHandler и пишет метрику.
/// </summary>
internal class ExecutionDurationMetricsInstrumentation : IDisposable
{
    private readonly ActivityListener _activityListener;

    public ExecutionDurationMetricsInstrumentation()
    {
        _activityListener = new ActivityListener
        {
            ActivityStopped = ActivityStopped,
            ShouldListenTo = activitySource => activitySource.Name == ExecutionHandler.ActivitySourceName
        };

        ActivitySource.AddActivityListener(_activityListener);
    }

    public void Dispose()
    {
        _activityListener.Dispose();
    }

    private static void ActivityStopped(Activity activity)
    {
        ExecutionDurationMeter.Record(activity.Duration.TotalMilliseconds,
            activity.GetTagItem("type") as string,
            activity.OperationName,
            activity.Status,
            activity.GetTagItem("result") as string);
    }
}