using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var request = context.Request;
        _logger.LogInformation("Incoming {Method} {Path}{QueryString}", request.Method, request.Path, request.QueryString);

        await _next(context);

        sw.Stop();
        var status = context.Response?.StatusCode;
        _logger.LogInformation("Handled {Method} {Path} responded {Status} in {Elapsed}ms",
            request.Method, request.Path, status, sw.ElapsedMilliseconds);
    }
}
