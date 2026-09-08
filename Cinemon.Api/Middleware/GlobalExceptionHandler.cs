using Cinemon.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cinemon.Api.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                BusinessRuleException => StatusCodes.Status400BadRequest,
                InvalidCredentialsException => StatusCodes.Status401Unauthorized,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            // Los errores de negocio (4xx) son parte del flujo normal, no algo roto en el servidor;
            // solo los 500 reales quedan como LogError.
            if (statusCode == StatusCodes.Status500InternalServerError) {
                _logger.LogError(exception, "An unhandled exception occurred.");
            } else {
                _logger.LogWarning("Business exception handled: {Message}", exception.Message);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    StatusCodes.Status400BadRequest => "Validation error",
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status404NotFound => "Resource not found",
                    StatusCodes.Status409Conflict => "Conflict",
                    _ => "An unexpected error occurred."
                },
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

            if (exception is ValidationException validationException) {
                problemDetails.Extensions["errors"] =
                    validationException.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(x => x.ErrorMessage).ToArray());
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
