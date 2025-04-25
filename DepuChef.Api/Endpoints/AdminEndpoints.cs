using DepuChef.Api.Models;
using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Services;
using DepuChef.Application.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DepuChef.Api.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminRoute = app.MapGroup("/admin");
        adminRoute.MapPost("/add-coins", AddCoins)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .WithName("AddCoins");

        adminRoute.MapDelete("/user/{userId}", DeleteUser);
    }

    private static async Task<IResult> AddCoins(
        [FromBody] AddCoinsRequest request,
        IUserService userService,
        IClaimsHelper claimsHelper,
        ILogger<AddCoinsRequest> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding coins to user.");
        
        var isAdminUser = await userService.IsAdmin(cancellationToken);
        if (!isAdminUser)
        {
            logger.LogWarning($"User not authorized to add coins");
            return Results.Unauthorized();
        }

        var user = await userService.GetUser(u => !u.IsArchived && u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            logger.LogWarning($"User not found with Id: {{{LogToken.UserId}}}", request.UserId);
            var error = new ProblemDetails
            {
                Title = "User not found.",
                Detail = "User not found."
            };
            return Results.NotFound(error);
        }

        user.VirtualCoins += request.Coins;
        await userService.UpdateUser(user, cancellationToken);
        return Results.Ok(
            new 
            {
                user.Id,
                Coins = user.VirtualCoins
            });
    }

    private static async Task<IResult> DeleteUser(
        Guid userId,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var isAdminUser = await userService.IsAdmin(cancellationToken);
        if (!isAdminUser)
        {
            return Results.Unauthorized();
        }

        await userService.DeleteUser(userId, cancellationToken);

        return Results.NoContent();
    }
}
