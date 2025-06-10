using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using FluentValidation;
using System;
using System.Net;
using System.Text.Json;

namespace BlogSystem.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            Result<object> response;
            int statusCode;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response = Result<object>.Failure(
                        message: "Validation failed.",
                        statusCode: statusCode,
                        errors: validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToList()
                    );
                    _logger.LogWarning(exception, "Validation error occurred.");
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = Result<object>.Failure(
                        message: "Unauthorized access.",
                        statusCode: statusCode,
                        errors: new List<string> { exception.Message }
                    );
                    _logger.LogWarning(exception, "Unauthorized access attempt.");
                    break;

                case NotFoundException notFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = Result<object>.Failure(
                        message: "Resource not found.",
                        statusCode: statusCode,
                        errors: new List<string> { exception.Message }
                    );
                    _logger.LogWarning(exception, "Resource not found.");
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = Result<object>.Failure(
                        message: "An unexpected error occurred.",
                        statusCode: statusCode,
                        errors: new List<string> { exception.Message }
                    );
                    _logger.LogError(exception, "Unexpected error occurred.");
                    break;
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
