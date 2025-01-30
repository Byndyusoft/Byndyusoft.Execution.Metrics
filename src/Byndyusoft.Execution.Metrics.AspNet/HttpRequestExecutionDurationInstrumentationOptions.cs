using Microsoft.AspNetCore.Http;

namespace Byndyusoft.Execution.Metrics.AspNet;

/// <summary>
///     Options for requests instrumentation.
/// </summary>
public class HttpRequestExecutionDurationInstrumentationOptions
{
    /// <summary>
    ///     Gets or sets a filter function that determines whether or not to
    ///     collect telemetry on a per request basis.
    /// </summary>
    /// <remarks>
    ///     Notes:
    ///     <list type="bullet">
    ///         <item>
    ///             The return value for the filter function is interpreted as:
    ///             <list type="bullet">
    ///                 <item>
    ///                     If filter returns <see langword="true" />, the request is
    ///                     collected.
    ///                 </item>
    ///                 <item>
    ///                     If filter returns <see langword="false" />, request is
    ///                     NOT collected.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </remarks>
    public Func<HttpContext, bool>? Filter { get; set; }
}