using DepuChef.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DepuChef.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        if (ex is InvalidClaimException invalidClaimException)
        {
            await HandleInvalidClaimException(context, invalidClaimException);
        }
        else
        {
            await HandleGenericException(context, ex);
        }

    }

    private async Task HandleGenericException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, "An error occurred.");

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred.",
            Detail = ex.Message
        };
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleInvalidClaimException(HttpContext context, InvalidClaimException invalidClaimException)
    {
        logger.LogError(invalidClaimException, "Invalid claim.");

        var problemDetails = new ProblemDetails
        {
            Title = "Invalid claim.",
            Detail = invalidClaimException.Message
        };
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
