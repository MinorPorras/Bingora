using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        var (statusCode, title, detail) = exception switch
        {
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Domain rule violation", exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, "Validation Error", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Key not found", exception.Message),
            DbUpdateException => (StatusCodes.Status409Conflict, "Database Update Error", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", exception.Message)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            detail = "Ocurrió un error inesperado. Por favor, inténtelo más tarde.";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}