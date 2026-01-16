using CartonCaps.Referrals.Common.Models;
using CartonCaps.Referrals.Domain.Dtos.V1;
using System.Net;
using System.Text.Json;

namespace CartonCaps.Referrals.ApiMock.Middlewares
{
    public class GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

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
            _logger.LogError(exception, "Unhandled exception");

            var (statusCode, apiError) = exception switch
            {
                RateLimitExceededException ex => (
                    HttpStatusCode.TooManyRequests,
                    new ApiErrorDto
                    (
                        Title: "Rate limit exceeded",
                        Detail: ex.Message,
                        Status: StatusCodes.Status429TooManyRequests,
                        Type: "rate_limit"
                    )
                ),

                VendorUnavailableException ex => (
                    HttpStatusCode.ServiceUnavailable,
                    new ApiErrorDto
                    (
                        Title: "External service unavailable",
                        Detail: ex.Message,
                        Status: StatusCodes.Status503ServiceUnavailable,
                        Type: "vendor_unavailable"
                    )
                ),

                NotFoundException ex => (
                    HttpStatusCode.NotFound,
                    new ApiErrorDto
                    (
                        Title: "Resource not found",
                        Detail: ex.Message,
                        Status: StatusCodes.Status404NotFound,
                        Type: "not_found"
                    )
                ),

                ConflictException ex => (
                    HttpStatusCode.Conflict,
                    new ApiErrorDto
                    (
                        Title: "Conflict",
                        Detail: ex.Message,
                        Status: StatusCodes.Status409Conflict,
                        Type: "conflict"
                    )
                ),

                ArgumentException ex => (
                    HttpStatusCode.BadRequest,
                    new ApiErrorDto
                    (
                        Title: "Invalid request",
                        Detail: ex.Message,
                        Status: StatusCodes.Status400BadRequest,
                        Type: "bad_request"
                    )
                ),

                InvalidOperationException ex => (
                    HttpStatusCode.BadRequest,
                    new ApiErrorDto
                    (
                        Title: "Invalid operation",
                        Detail: ex.Message,
                        Status: StatusCodes.Status400BadRequest,
                        Type: "bad_request"
                    )
                ),

                _ => (
                    HttpStatusCode.InternalServerError,
                    new ApiErrorDto
                    (
                        Title: "Internal server error",
                        Detail: "An unexpected error occurred.",
                        Status: StatusCodes.Status500InternalServerError,
                        Type: "internal_error"
                    )
                )
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(apiError);
            await context.Response.WriteAsync(json);
        }
    }
}
