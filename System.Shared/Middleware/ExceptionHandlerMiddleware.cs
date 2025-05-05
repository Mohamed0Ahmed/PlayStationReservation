using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using System.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace System.Shared.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            catch (CustomException ex)
            {
                _logger.LogWarning("Custom exception occurred: {Message}", ex.Message);
                await HandleCustomExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse<object>("حدث خطأ غير متوقع: " + exception.Message, 500);
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;

            var response = new ApiResponse<object>(exception.Message, exception.StatusCode);
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
