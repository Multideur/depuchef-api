using DepuChef.Api.Models;
using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Models.User;
using DepuChef.Application.Services;
using DepuChef.Api.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using DepuChef.Application.Utilities;

namespace DepuChef.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var userRoute = app.MapGroup("/user");
        userRoute.MapPost("/register", RegisterUser)
            .RequireAuthorization()
            .Produces<UserResponse>()
            .ProducesValidationProblem();

        userRoute.MapGet("", GetUser)
            .RequireAuthorization()
            .Produces<UserResponse>();

        userRoute.MapGet("/{userId}/recipe", GetUserRecipes)
            .RequireAuthorization()
            .Produces<List<RecipeResponse>>();

        userRoute.MapPut("/{userId}", UpdateUser)
            .RequireAuthorization()
            .Produces<UserResponse>();

        userRoute.MapPatch("/{userId}/recipe/{recipeId}", UpdateUserRecipeFavourite)
            .RequireAuthorization();
    }

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        IValidator<RegisterUserRequest> validator,
        IUserService userService,
        ILogger<RegisterUserRequest> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"Registering user.");

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.ToDictionary();
                LoggingHelper.LogCollectionValues(validationErrors.Values, logger);
                return Results.ValidationProblem(validationErrors);
            }

            var result = await userService.RegisterUser(request, cancellationToken);
            if (result == null)
            {
                return Results.BadRequest();
            }

            var userResponse = new UserResponse
            {
                Id = result.Id,
                Email = result.Email,
                FirstName = result.FirstName,
                LastName = result.LastName,
                VirtualCoins = result.VirtualCoins,
                SubscriptionLevel = result.SubscriptionLevel,
                ChefPreference = result.ChefPreference
            };

            return Results.Ok(userResponse);
        }
        catch (InvalidClaimException ex)
        {
            logger.LogError(ex, $"Invalid claim exception. Claim type: {{{LogToken.ClaimType}}}", ex.ClaimType);
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid claim.",
                Detail = ex.Message + $" {ex.ClaimType}"
            };

            return Results.BadRequest(problemDetails);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Invalid operation exception.");
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid operation.",
                Detail = ex.Message
            };

            return Results.BadRequest(problemDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user.");
            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> UpdateUser(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        IUserService userService,
        ILogger<UpdateUserRequest> logger,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUser(u => !u.IsArchived && u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.ChefPreference = ChefChoice.FromValue(request.ChefPreference);

        await userService.UpdateUser(user, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetUser(
        IClaimsHelper claimsHelper,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        claimsHelper.CheckClaims(out _, out var email);
        var user = await userService.GetUser(u => !u.IsArchived && u.Email == email, cancellationToken);
        if (user == null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "User not found.",
                Detail = "User does not exist or is archived."
            };
            return Results.NotFound(problemDetails);
        }
        var userResponse = new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            VirtualCoins = user.VirtualCoins,
            SubscriptionLevel = user.SubscriptionLevel,
            ChefPreference = user.ChefPreference
        };
        return Results.Ok(userResponse);
    }

    private static async Task<IResult> GetUserRecipes(
         Guid userId,
         IUserService userService,
         IRecipeService recipeService,
         CancellationToken cancellationToken)
    {
        var user = await userService.GetUser(u => !u.IsArchived && u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }
        var recipes = await recipeService.GetRecipesForUser(userId, cancellationToken);
        if (recipes == null)
        {
            return Results.NotFound();
        }
        var recipeResponses = recipes.Select(r => new RecipeResponse
        {
            Id = r.Id,
            Title = r.Title,
            IsFavourite = r.IsFavourite,
            Ingredients = r.Ingredients?.Select(i => new IngredientDto
            {
                Category = i.Category,
                Items = i.Items?.Select(i => new IngredientItemDto
                {
                    Name = i.Name,
                    Calories = i.Calories
                }).ToList(),
                Calories = i.Calories,
                HealthySubstitutions = i.HealthySubstitutions?.Select(h => new HealthySubstitutionDto
                {
                    Original = h.Original,
                    Substitute = h.Substitute
                }).ToList(),
                CaloriesAfterSubstitution = i.CaloriesAfterSubstitution
            }).ToList(),
            Instructions = r.Instructions?.Select(i => new InstructionDto
            {
                Step = i.Step,
                Description = i.Description
            }).ToList(),
            Confidence = r.Confidence,
            CookTime = r.CookTime,
            Description = r.Description,
            Notes = r.Notes?.Select(n => new NoteDto
            {
                Text = n.Text
            }).ToList(),
            PrepTime = r.PrepTime,
            Rating = r.Rating,
            Servings = r.Servings,
            TotalTime = r.TotalTime,
            Calories = r.Calories,
            CaloriesAfterSubstitution = r.CaloriesAfterSubstitution
        }).ToList();

        return Results.Ok(recipeResponses);
    }

    private static async Task<IResult> UpdateUserRecipeFavourite(
        IRecipeService recipeService,
        Guid userId,
        Guid recipeId,
        [FromBody] UpdateFavouriteDto updateFavouriteDto,
        CancellationToken cancellationToken)
    {
        var result = await recipeService.UpdateRecipeFavourite(userId,
            recipeId,
            updateFavouriteDto.IsFavourite,
            cancellationToken);

        if (result == null)
        {
            return Results.NotFound();
        }
        return Results.Ok();
    }
}