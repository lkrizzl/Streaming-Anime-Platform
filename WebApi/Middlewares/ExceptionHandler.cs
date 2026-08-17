using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middlewares;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Title = "NotFound",
                Detail = notFound.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                Status = StatusCodes.Status404NotFound,
            },
            BadRequestException badRequest => new ProblemDetails
            {
                Title = "BadRequest",
                Detail = badRequest.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", badRequest.Error }
                }
            },
            ForbiddenException forbidden => new ProblemDetails
            {
                Title = "Forbidden",
                Detail = forbidden.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                Status = StatusCodes.Status403Forbidden,
            },
            ValidationException validation => new ProblemDetails
            {
                Title = "BadRequest",
                Detail = ValidationException.ValidationErrorMessage,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", validation.Errors }
                }
            },
            FluentValidation.ValidationException fluentValidation => new ProblemDetails
            {
                Title = "ValidationError",
                Detail = "One or more validation errors occurred.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", fluentValidation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    }).ToList() }
                }
            },
            _ => new ProblemDetails
            {
                Title = "InternalServerError",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
            }
        };

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception ({StatusCode}) while processing {Method} {Path}: {Message}",
                problemDetails.Status, httpContext.Request.Method, httpContext.Request.Path, exception.Message);
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}