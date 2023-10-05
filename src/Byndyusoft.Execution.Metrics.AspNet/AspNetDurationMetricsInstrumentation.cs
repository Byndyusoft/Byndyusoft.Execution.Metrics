namespace Byndyusoft.Execution.Metrics.AspNet;

using System.Diagnostics;

/// <summary>
///     Инструментация с получением метрик входящих http запросов
/// </summary>
public class AspNetDurationMetricsInstrumentation : IDisposable
{
    private readonly IDisposable _subscription;

    public AspNetDurationMetricsInstrumentation()
    {
        var observer = new AspNetCoreDiagnosticObserver();
        _subscription = DiagnosticListener.AllListeners.Subscribe(observer);
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }
}