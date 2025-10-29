using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tekno.Api.Middlewares
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseWrapperMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ⛔️ Bỏ qua Swagger, healthcheck, file tĩnh
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/swagger") || path.StartsWith("/health") || path.StartsWith("/favicon"))
            {
                await _next(context);
                return;
            }

            var originalBody = context.Response.Body;
            await using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả lại stream gốc cho ExceptionMiddleware xử lý
                _logger.LogError(ex, "Error before response wrapping.");
                context.Response.Body = originalBody;
                throw;
            }

            // Nếu statusCode không phải 2xx, không wrap
            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                return;
            }

            // Đọc nội dung body
            memStream.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(memStream).ReadToEndAsync();

            // Nếu không phải JSON → bỏ qua
            if (string.IsNullOrWhiteSpace(context.Response.ContentType) ||
                !context.Response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                memStream.Seek(0, SeekOrigin.Begin);
                await memStream.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                return;
            }

            // Nếu body rỗng → wrap mặc định
            if (string.IsNullOrWhiteSpace(bodyText))
            {
                bodyText = "{}";
            }

            // Kiểm tra nếu đã có property "success" → không wrap lại
            bool alreadyWrapped = false;
            try
            {
                using var doc = JsonDocument.Parse(bodyText);
                alreadyWrapped = doc.RootElement.TryGetProperty("success", out _);
            }
            catch (JsonException)
            {
                // ignore
            }

            string output;
            if (alreadyWrapped)
            {
                output = bodyText;
            }
            else
            {
                object? parsedData = null;
                try
                {
                    parsedData = JsonSerializer.Deserialize<object>(bodyText, _jsonOptions);
                }
                catch
                {
                    parsedData = bodyText;
                }

                var wrapped = new
                {
                    success = true,
                    message = "Success",
                    data = parsedData,
                    timestamp = DateTime.UtcNow
                };

                output = JsonSerializer.Serialize(wrapped, _jsonOptions);
            }

            context.Response.Body = originalBody;
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(output);
            await context.Response.WriteAsync(output);
        }
    }
}
