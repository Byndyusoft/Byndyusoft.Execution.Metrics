using Byndyusoft.Execution.Metrics.Hangfire;
using Hangfire;
using Hangfire.Annotations;

namespace Byndyusoft.Execution.Metrics;

public static class HangfireConfigurationExtensions
{
    /// <summary>
    ///     Добавить фильтр для получения метрик выполнения операций Hangfire
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IGlobalConfiguration<HangfireExecutionMetricDurationFilter> AddExecutionMetricDurationFilter(
        [NotNull] this IGlobalConfiguration configuration)
    {
        return configuration.UseFilter(new HangfireExecutionMetricDurationFilter());
    }
}