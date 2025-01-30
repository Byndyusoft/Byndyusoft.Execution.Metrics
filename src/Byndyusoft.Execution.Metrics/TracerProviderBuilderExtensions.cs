using OpenTelemetry.Trace;

namespace Byndyusoft.Execution.Metrics;

public static class TracerProviderBuilderExtensions
{
    /// <summary>
    ///     Добавляет сбор данных трассировки запущенной через ExecutionHandler
    /// </summary>
    public static TracerProviderBuilder AddExecutionDurationInstrumentation(
        this TracerProviderBuilder builder)
    {
        return builder.AddSource(ExecutionHandler.ActivitySourceName);
    }
}