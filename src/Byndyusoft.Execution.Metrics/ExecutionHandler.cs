namespace Byndyusoft.Execution.Metrics;

using System;
using System.Diagnostics;
using OpenTelemetry.Trace;


/// <summary>
///     Создаёт Activity, необходимое для замера времени выполнения запросов и их трассировки
/// </summary>
public class ExecutionHandler : IDisposable
{
    public static readonly string ActivitySourceName = "Byndyusoft.Execution.Metrics";
    public static readonly ActivitySource ExecutionMetricsSource = new(ActivitySourceName);
    private bool _hasResult;

    public Activity? Activity { get; }

    public ExecutionHandler(string type, string operationName, ActivityContext? parent = null)
    {
        Activity = ExecutionMetricsSource.StartActivity(operationName, ActivityKind.Internal, parent ?? default);
        Activity?.AddTag("type", type);
    }

    public void Dispose()
    {
        Activity?.Dispose();
    }

    public void SetOkResult(string? result = null)
    {
        if (_hasResult)
            return;

        Activity?.SetStatus(ActivityStatusCode.Ok);
        Activity?.SetTag("result", result);

        _hasResult = true;
    }

    public void SetErrorResult(Exception? exception = null, string? result = null)
    {
        if (_hasResult)
            return;

        Activity?.SetStatus(ActivityStatusCode.Error);
        Activity?.SetTag("result", result);

        Activity?.RecordException(exception);

        _hasResult = true;
    }

    public static void Execute(string type, string name, Func<Activity?, string?> action, ActivityContext? parent = null)
    {
        using var executionHandler = new ExecutionHandler(type, name, parent);
        try
        {
            var result = action(executionHandler.Activity);

            executionHandler.SetOkResult(result);
        }
        catch (Exception e)
        {
            executionHandler.SetErrorResult(e);
        }
    }
}