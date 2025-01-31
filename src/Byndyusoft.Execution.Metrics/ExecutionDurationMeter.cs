using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Byndyusoft.Execution.Metrics;

/// <summary>
///     Предоставляет доступ к записи метрики длительности выполнения.
///     Нужен для написания собственных адаптеров записывающих метрику
/// </summary>
public static class ExecutionDurationMeter
{
    public static readonly string Name = "Byndyusoft.Execution.Meter";

    private static readonly Meter Meter = new(Name);
    private static readonly Histogram<double> Instrument = Meter.CreateHistogram<double>("execution.duration", "ms");

    public static void Record(double duration, string? type, string? operationName, ActivityStatusCode statusCode,
        string? result)
    {
        Instrument.Record(duration,
            new TagList
            {
                new("type", type),
                new("operation", operationName),
                new("status_code", statusCode switch
                {
                    ActivityStatusCode.Ok => "OK",
                    ActivityStatusCode.Error => "ERROR",
                    _ => ""
                }),
                new("result", result ?? "")
            });
    }
}