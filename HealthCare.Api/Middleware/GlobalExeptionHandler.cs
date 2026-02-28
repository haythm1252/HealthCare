using FluentValidation; // Make sure this is added
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Middleware;

public class GlobalExeptionHandler(ILogger<GlobalExeptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExeptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Check if the error is a Validation Failure
        if (exception is ValidationException validationException)
        {
            _logger.LogWarning("Validation failed: {Message}", exception.Message);

            var validationProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation failures have occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            // Convert FluentValidation errors to a dictionary for the response
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            validationProblemDetails.Extensions.Add("errors", errors);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);

            return true; // Error handled
        }

        // 2. Otherwise, it's a real server error (500)
        _logger.LogError(exception, "Something went wrong: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = "An unexpected error occurred on our end.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}