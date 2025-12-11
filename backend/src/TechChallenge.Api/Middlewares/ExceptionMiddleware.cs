using FluentValidation;
using System.Net;
using System.Text.Json;
using TechChallenge.Domain.Exceptions;

namespace TechChallenge.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var fieldError = ex.Errors.FirstOrDefault()
                ?? throw new Exception(ex.Message);

            _logger.LogError(ex, "Error in validation of the {PropertyName}: {ErrorMessage}", fieldError.PropertyName, fieldError.ErrorMessage);

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.Headers.ContentLength = null;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = fieldError.CustomState.ToString(),
                message = fieldError.ErrorMessage,
                field = fieldError.PropertyName
            }));
        }
        catch (DomainValidationException ex)
        {
            _logger.LogError(ex, "Error in domain validation: {Message}", ex.Message);

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            context.Response.Headers.ContentLength = null;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = ex.ErrorType.ToString(),
                message = ex.Message,
                trace = ex.StackTrace
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal server error");

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started. It is not possible to write the error body.");
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.Headers.ContentLength = null;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "INTERNAL_SERVER_ERROR",
                message = "Internal server error, please contact of system admin",
                trace = ex.StackTrace
            }));
        }
    }
}
