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
        this MeterProviderBuilder builder,
        Action<HttpRequestExecutionDurationInstrumentationOptions>? configureOptions)
    {
        builder.AddMeter(ExecutionDurationMeter.Name);

        var options = new HttpRequestExecutionDurationInstrumentationOptions();
        configureOptions?.Invoke(options);

        return builder.AddInstrumentation(() => new HttpRequestExecutionDurationInstrumentation(options));
    }
}