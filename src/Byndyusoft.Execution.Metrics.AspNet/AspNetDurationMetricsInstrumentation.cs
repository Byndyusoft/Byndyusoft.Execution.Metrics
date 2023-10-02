namespace Byndyusoft.Execution.Metrics.AspNet;

using System.Diagnostics;

public class AspNetDurationMetricsInstrumentation :IDisposable
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