using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Master.API.Middleware
{
    /// <summary>
    /// Middleware for centralized exception handling across the application.
    /// Converts exceptions into standardized HTTP responses using ProblemDetails.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GlobalExceptionMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">Logger for recording exception details.</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to handle exceptions.
        /// </summary>
        /// <param name="context">HTTP context.</param>
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

        /// <summary>
        /// Handles the exception and writes a standardized ProblemDetails response.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request");

            context.Response.ContentType = "application/problem+json";

            var (statusCode, problem) = ex switch
            {
                ValidationException validationException =>
                    (400, CreateValidationProblemDetails(context, validationException, 400)),

                KeyNotFoundException =>
                    (404, CreateProblemDetails(context, 404, "Resource not found", ex.Message)),

                ArgumentException =>
                    (400, CreateProblemDetails(context, 400, "Invalid request", ex.Message)),

                InvalidOperationException =>
                    (400, CreateProblemDetails(context, 400, "Invalid request", ex.Message)),

                UnauthorizedAccessException =>
                    (401, CreateProblemDetails(context, 401, "User unauthorized", ex.Message)),

                _ =>
                    (500, CreateProblemDetails(context, 500, "An unexpected error occurred", ex.Message))
            };

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(problem);

            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Creates a standard <see cref="ProblemDetails"/> object.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="title">Title of the problem.</param>
        /// <param name="detail">Detailed message.</param>
        /// <returns>Configured <see cref="ProblemDetails"/> object.</returns>
        private ProblemDetails CreateProblemDetails(HttpContext context, int statusCode, string title, string detail)
        {
            return new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{statusCode}",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };
        }

        /// <summary>
        /// Creates a <see cref="ProblemDetails"/> object specifically for FluentValidation errors.
        /// Includes validation errors in the 'errors' extension property.
        /// </summary>
        /// <param name="context">HTTP context.</param>
        /// <param name="validationException">Validation exception that occurred.</param>
        /// <param name="statusCode">HTTP status code (usually 400).</param>
        /// <returns>Configured <see cref="ProblemDetails"/> object with validation errors.</returns>
        private ProblemDetails CreateValidationProblemDetails(HttpContext context, ValidationException validationException, int statusCode)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                Title = "One or more validation errors occurred",
                Status = statusCode,
                Detail = "See the 'errors' property for more details",
                Instance = context.Request.Path
            };

            problem.Extensions["errors"] = errors;

            return problem;
        }
    }
}