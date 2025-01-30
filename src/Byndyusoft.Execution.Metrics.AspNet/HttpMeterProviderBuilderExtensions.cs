using Byndyusoft.Execution.Metrics.AspNet;
using OpenTelemetry.Metrics;

// ReSharper disable once CheckNamespace
namespace Byndyusoft.Execution.Metrics;

public static class HttpMeterProviderBuilderExtensions
{
    /// <summary>
    ///     Добавляет метрики длительности выполнения входящих http-запросов
    /// </summary>
    public static MeterProviderBuilder AddHttpRequestExecutionDurationInstrumentation(
        this MeterProviderBuilder builder)
    {
        builder.AddMeter(ExecutionDurationMeter.Name);
        return builder.AddInstrumentation(() => new HttpRequestExecutionDurationInstrumentation());
    }
}