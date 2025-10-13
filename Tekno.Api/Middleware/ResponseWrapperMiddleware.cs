using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only process JSON responses for non-error status codes
        var originalBodyStream = context.Response.Body;

        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var contentType = context.Response.ContentType ?? string.Empty;

        // If not JSON or empty body (204), just copy through
        if (!contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(bodyText)
            || context.Response.StatusCode >= 400) // errors handled by ErrorHandlingMiddleware
        {
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
            return;
        }

        try
        {
            // Check if body is already ApiResponse by parsing top-level "success" property
            using var doc = JsonDocument.Parse(bodyText);
            if (doc.RootElement.TryGetProperty("success", out _))
            {
                // Already wrapped; return as-is
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
                return;
            }
        }
        catch (JsonException)
        {
            // If parse fails, treat as raw and wrap
        }

        // Wrap the raw JSON payload in ApiResponse<object>
        object? originalData = null;
        try
        {
            originalData = JsonSerializer.Deserialize<object>(bodyText, _jsonOptions);
        }
        catch
        {
            originalData = bodyText; // fallback to raw string
        }

        var wrapped = new
        {
            success = true,
            message = "Success",
            data = originalData,
            timestamp = DateTime.UtcNow
        };

        var wrappedJson = JsonSerializer.Serialize(wrapped, _jsonOptions);
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(wrappedJson);
        context.Response.Body = originalBodyStream;
        await context.Response.WriteAsync(wrappedJson);
    }
}
