using BlogSystem.Domain.Common;
using BlogSystem.Domain.Exceptions;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
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

                case SqlException sqlEx:
                    switch (sqlEx.Number)
                    {
                        case 2601: // duplicate key
                        case 2627: // Primary key violation
                            statusCode = StatusCodes.Status409Conflict;
                            response = Result<object>.Failure(
                                new List<string> { "A record with the same unique identifier already exists." },
                                statusCode,
                                "Duplicate Entry");
                            _logger.LogError(sqlEx, "SqlException (Duplicate Entry): {Message}", sqlEx.Message);
                            break;
                        case 1205: // Deadlock victim
                            statusCode = StatusCodes.Status409Conflict; // Or 500
                            response = Result<object>.Failure(
                                new List<string> { "The operation could not be completed due to a database deadlock. Please try again." },
                                statusCode,
                                "Database Conflict");
                            _logger.LogError(sqlEx, "SqlException (Deadlock): {Message}", sqlEx.Message);
                            break;
                        case -2: // Timeout expired
                        case 2:  // SQL Server did not find the server (connection error)
                        case 53: // A network-related or instance-specific error occurred while establishing a connection to SQL Server.
                            statusCode = StatusCodes.Status500InternalServerError;
                            response = Result<object>.Failure(
                                new List<string> { "A transient issue occurred while connecting to the database. Please try again shortly." },
                                statusCode,
                                "Database Connection Issue");
                            _logger.LogError(sqlEx, "SqlException (Connection/Timeout): {Message}", sqlEx.Message);
                            break;
                        default:
                            // Generic SQL Server error
                            statusCode = StatusCodes.Status500InternalServerError;
                            response = Result<object>.Failure(
                                new List<string> { "An unexpected database error occurred. Please contact support." },
                                statusCode,
                                "Database Error");
                            _logger.LogError(sqlEx, "SqlException (Unhandled Code {Code}): {Message}", sqlEx.Number, sqlEx.Message);
                            break;
                    }
                    break;

                case DatabaseConnectionException customDbConnEx:
                    statusCode = StatusCodes.Status500InternalServerError;
                    response = Result<object>.Failure(
                        new List<string> { "A problem occurred while connecting to the database. Please try again later." },
                        statusCode,
                        "Database Connection Error");
                    _logger.LogError(customDbConnEx, "DatabaseConnectionException: {Message}", customDbConnEx.Message);
                    break;

                case DatabaseOperationException dbOpEx:
                    statusCode = StatusCodes.Status500InternalServerError;
                    response = Result<object>.Failure(
                        new List<string> { "An unexpected database operation error occurred. Please contact support." },
                        statusCode,
                        "Database Operation Error");
                    _logger.LogError(dbOpEx, "DatabaseOperationException: {Message}", dbOpEx.Message);
                    break;

                case DbUpdateConcurrencyException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    response = Result<object>.Failure(
                        message: "Database concurrency error.",
                        statusCode: statusCode,
                        errors: new List<string> { "The record you attempted to update was modified by another user. Please refresh and try again." }
                    );
                    _logger.LogWarning(exception, "Database concurrency conflict occurred.");
                    break;

                case DatabaseConstraintViolationException constraintViolationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    response = Result<object>.Failure(
                        new List<string> { constraintViolationEx.Message },
                        statusCode,
                        "Invalid Data");
                    _logger.LogWarning("DatabaseConstraintViolationException: {Message}", constraintViolationEx.Message);
                    break;

                case DuplicateEntryException duplicateEx: 
                    statusCode = StatusCodes.Status409Conflict;
                    response = Result<object>.Failure(
                        new List<string> { duplicateEx.Message },
                        statusCode,
                        "Duplicate Entry");
                    _logger.LogWarning("DuplicateEntryException: {Message}", duplicateEx.Message);
                    break;

                case DbException dbException:
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    response = Result<object>.Failure(
                        message: "Database error occurred.",
                        statusCode: statusCode,
                        errors: new List<string> { "An error occurred while accessing the database. Please try again later." }
                    );
                    _logger.LogError(exception, "General database error occurred.");
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
