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
    }

    private static async Task<IResult> AddCoins(
        [FromBody] AddCoinsRequest request,
        IUserService userService,
        IClaimsHelper claimsHelper,
        ILogger<AddCoinsRequest> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding coins to user.");

        CheckClaims(claimsHelper, out string? _, out string? emailClaim);

        if (!Information.AdminUsers.Contains(emailClaim))
        {
            logger.LogWarning($"User not authorized to add coins: {{{LogToken.Email}}}", emailClaim);
            return Results.Unauthorized();
        }

        var user = await userService.GetUser(u => u.Id == request.UserId, cancellationToken);
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

    private static void CheckClaims(IClaimsHelper claimsHelper, out string? authUserId, out string? emailClaim)
    {
        var claims = claimsHelper.RetrieveClaims() ?? throw new Exception("Claims are required.");
        authUserId = claims.SingleOrDefault(claim => claim.Type == ClaimType.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(authUserId))
            throw new InvalidClaimException(ClaimType.Sub);
        emailClaim = claims.SingleOrDefault(claim => claim.Type == ClaimType.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaim))
            throw new InvalidClaimException(ClaimType.Email);
    }
}
