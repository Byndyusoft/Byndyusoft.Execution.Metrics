namespace Byndyusoft.Execution.Metrics.Hangfire;

using global::Hangfire.Server;

/// <summary>
///     Фильтр, который собирает трассировку и время выполнения запущенных заданий
/// </summary>
public class HangfireExecutionMetricDurationFilter : IServerFilter
{
    private const string HandlerKeyName = "handler";

    public void OnPerforming(PerformingContext filterContext)
    {
        var recurringJobId = filterContext.Connection.GetJobParameter(filterContext.BackgroundJob.Id, "RecurringJobId");

        var handler = new ExecutionHandler("hangfire", recurringJobId ?? filterContext.BackgroundJob.Job.Type.Name);
        filterContext.Items.Add(HandlerKeyName, handler);
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        var handler = (ExecutionHandler)filterContext.Items[HandlerKeyName];

        if (filterContext.Exception != null)
            handler.SetErrorResult(filterContext.Exception);
        else
            handler.SetOkResult();

        handler.Dispose();
    }
}