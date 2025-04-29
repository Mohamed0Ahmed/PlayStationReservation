using Microsoft.AspNetCore.Http;
using System.Shared.Exceptions;
using Microsoft.AspNetCore.Builder;

namespace System.Shared.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomException ex)
            {
                await HandleCustomExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
        {
            context.Items["ErrorMessage"] = exception.Message;
            return _next(context);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Items["ErrorMessage"] = "An unexpected error occurred: " + exception.Message;
            return _next(context); 
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