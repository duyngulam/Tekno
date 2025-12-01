using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Tekno.Application.Common.Exceptions;

namespace Tekno.Api.Middlewares 
{ 
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                _logger.LogWarning("ExceptionMiddleware caught: {Type}", ex.GetType().Name);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred.";
            string errorCode = "SERVER_ERROR";
            object? details = null;

            switch (ex)
            {
                // Handle custom application exceptions
                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    message = appEx.Message;
                    errorCode = appEx.ErrorCode;
                    if (appEx is ValidationException valEx)
                        details = valEx.Errors;
                    break;

                // Handle invalid operation exceptions from services
                case InvalidOperationException invalidOpEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    errorCode = "INVALID_OPERATION";
                    break;

                // Handle argument exceptions
                case ArgumentException argEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = argEx.Message;
                    errorCode = "INVALID_ARGUMENT";
                    break;

                // Handle database duplicate key exceptions
                case DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("duplicate") == true:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = "A record with the same unique identifier already exists.";
                    errorCode = "DB_DUPLICATE";
                    details = new { innerMessage = dbEx.InnerException?.Message };
                    break;

                // Handle database foreign key exceptions
                case DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("foreign key") == true:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Referenced record does not exist or cannot be deleted due to dependencies.";
                    errorCode = "DB_FOREIGN_KEY";
                    details = new { innerMessage = dbEx.InnerException?.Message };
                    break;

                // Handle other database exceptions
                case DbUpdateException dbEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Database operation failed. " + dbEx.Message;
                    errorCode = "DB_UPDATE_ERROR";
                    details = new { innerMessage = dbEx.InnerException?.Message };
                    break;

                // Handle JSON deserialization exceptions
                case JsonException jsonEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Invalid JSON format in request body.";
                    errorCode = "INVALID_JSON";
                    details = new { error = jsonEx.Message };
                    break;

                // Handle unauthorized access
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    message = "You do not have permission to perform this action.";
                    errorCode = "FORBIDDEN";
                    break;

                // Default internal server error
                default:
                    // For other exceptions, log the full stack trace but don't expose it to client
                    details = new { type = ex.GetType().Name };
                    break;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                statusCode,
                errorCode,
                message,
                details,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}