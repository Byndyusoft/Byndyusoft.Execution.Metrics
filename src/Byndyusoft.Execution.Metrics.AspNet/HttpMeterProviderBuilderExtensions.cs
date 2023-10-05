namespace Byndyusoft.Execution.Metrics;

using AspNet;
using OpenTelemetry.Metrics;

public static class HttpMeterProviderBuilderExtensions
{
    /// <summary>
    ///     Добавляет метрики длительности выполнения входящих http-запросов
    /// </summary>
    public static MeterProviderBuilder AddHttpRequestExecutionDurationInstrumentation(
        this MeterProviderBuilder builder)
    {
        builder.AddMeter(ExecutionDurationMeter.Name);
        return builder.AddInstrumentation(() => new AspNetDurationMetricsInstrumentation());
    }
}