﻿using OpenTelemetry.Metrics;

namespace Byndyusoft.Execution.Metrics;

public static class MeterProviderBuilderExtensions
{
    /// <summary>
    ///     Добавляет метрики длительности выполнения запущенные через ExecutionHandler
    /// </summary>
    public static MeterProviderBuilder AddExecutionDurationInstrumentation(
        this MeterProviderBuilder builder)
    {
        builder.AddMeter(ExecutionDurationMeter.Name);
        return builder.AddInstrumentation(() => new ExecutionDurationMetricsInstrumentation());
    }
}