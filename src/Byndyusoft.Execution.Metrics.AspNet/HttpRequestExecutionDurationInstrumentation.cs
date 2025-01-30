using System.Diagnostics;

namespace Byndyusoft.Execution.Metrics.AspNet;

/// <summary>
///     Инструментация с получением метрик входящих http запросов
/// </summary>
public class HttpRequestExecutionDurationInstrumentation : IDisposable
{
    private readonly IDisposable _subscription;

    public HttpRequestExecutionDurationInstrumentation()
    {
        var observer = new AspNetCoreDiagnosticObserver();
        _subscription = DiagnosticListener.AllListeners.Subscribe(observer);
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}