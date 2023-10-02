namespace Byndyusoft.Execution.Metrics;

using OpenTelemetry.Trace;

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