using FluentValidation;
using System.Text.Json;

namespace TaskManagement.Server.Middleware
{
    /// <summary>
    /// Middleware for handling FluentValidation errors
    /// </summary>
    public class ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        /// <summary>
        /// Handles validation exceptions from FluentValidation
        /// </summary>
        /// <param name="context">HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning("Validation failed: {ValidationErrors}",
                    string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));

                await HandleValidationExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Converts validation exceptions to HTTP 400 responses
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Validation exception</param>
        private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "Validation failed",
                errors = exception.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    message = e.ErrorMessage
                }),
                timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
