﻿namespace Byndyusoft.Execution.Metrics.AspNet;

using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
///     Снимает метрики длительности выполнения входящих http-запросов
/// </summary>
public sealed class AspNetCoreDiagnosticObserver : IObserver<DiagnosticListener>,
                                                   IObserver<KeyValuePair<string, object?>>
{
    private readonly List<IDisposable> _subscriptions = new();

    void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
    {
        if (diagnosticListener.Name == "Microsoft.AspNetCore")
        {
            var subscription = diagnosticListener.Subscribe(this);
            _subscriptions.Add(subscription);
        }
    }

    void IObserver<DiagnosticListener>.OnError(Exception error)
    {
    }

    void IObserver<DiagnosticListener>.OnCompleted()
    {
        _subscriptions.ForEach(x => x.Dispose());
        _subscriptions.Clear();
    }

    void IObserver<KeyValuePair<string, object?>>.OnCompleted()
    {
    }

    void IObserver<KeyValuePair<string, object?>>.OnError(Exception error)
    {
    }

    void IObserver<KeyValuePair<string, object?>>.OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key != "Microsoft.AspNetCore.Hosting.HttpRequestIn.Stop")
            return;

        var activity = Activity.Current;
        if (activity == null)
            return;

        if (value.Value is not HttpContext context)
            return;

        if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("metrics"))
            return;

        var endpoint = context.GetEndpoint();
        var target = (endpoint as RouteEndpoint)?.RoutePattern.RawText ?? endpoint?.DisplayName ?? context.Request.Path;
        var name = context.Request.Method == HttpMethods.Options
                       ? context.Request.Method
                       : context.Request.Method + " " + target;

        ExecutionDurationMeter.Record(activity.Duration.TotalMilliseconds,
                                      "http",
                                      name,
                                      context.Response.StatusCode is >= 500 and < 600
                                          ? ActivityStatusCode.Error 
                                          : ActivityStatusCode.Ok ,
                                      context.Response.StatusCode.ToString());
    }
}