using Microsoft.EntityFrameworkCore;
using System.Net;
using Tekno.Application.Common.Exceptions;
namespace Tekno.Api.Middlewares { 
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
                _logger.LogError(ex, "Unhandled exceptioccccccccccccccccccccccccn: {Message}", ex.Message);
                _logger.LogWarning("🎯 ExceptionMiddleware caught: {Type}", ex.GetType().Name);
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
                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    message = appEx.Message;
                    errorCode = appEx.ErrorCode;
                    if (appEx is ValidationException valEx)
                        details = valEx.Errors;
                    break;

                case DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("duplicate") == true:
                    statusCode = 409;
                    message = "Duplicate record detected.";
                    errorCode = "DB_DUPLICATE";
                    break;

                case DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("foreign key") == true:
                    statusCode = 400;
                    message = "Foreign key constraint violation.";
                    errorCode = "DB_FOREIGN_KEY";
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
                details
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}