using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Tekno.Api.Common.Exceptions;
using Tekno.Api.Common.Responses;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
    {
        int statusCode = (int)HttpStatusCode.InternalServerError;
        object? errors = null;
        string message = "An unexpected error occurred.";

        switch (ex)
        {
            case AppException ae:
                statusCode = ae.StatusCode;
                message = ae.Message;
                errors = ae.Errors;
                break;
            case KeyNotFoundException _:
                statusCode = (int)HttpStatusCode.NotFound;
                message = ex.Message;
                break;
            // add more domain-specific exceptions if needed
            default:
                // leave 500 and generic message
                message = ex.Message; // optionally hide in production
                break;
        }

        logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = ApiResponse<string>.Fail(message, errors);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
