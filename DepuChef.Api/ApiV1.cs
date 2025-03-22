using DepuChef.Api.Models;
using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Models;
using DepuChef.Application.Models.User;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DepuChef.Api;

public static class ApiV1
{
    public static void AddEndpoints(this WebApplication app)
    {
        var recipeRoute = app.MapGroup("/recipe");
        recipeRoute.MapPost("/create", CreateRecipeFromImage)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("GenerateRecipe")
            .WithOpenApi();

        recipeRoute.MapGet("/{processId}", GetRecipeFromProcess)
            .Produces<RecipeResponse>()
            .RequireAuthorization();

        var userRoute = app.MapGroup("/user");
        userRoute.MapPost("/register", RegisterUser)
            .Produces<UserResponse>()
            .ProducesValidationProblem()
            .RequireAuthorization();

        userRoute.MapPut("/{id}", UpdateUser)
            .Produces<UserResponse>()
            .RequireAuthorization();

        userRoute.MapGet("/{id}", GetUser)
            .Produces<UserResponse>()
            .RequireAuthorization();

        userRoute.MapGet("/{userId}/recipe", GetUserRecipes)
            .Produces<List<RecipeResponse>>()
            .RequireAuthorization();

        userRoute.MapPatch("/{userId}/recipe/{recipeId}", UpdateUserRecipeFavourite)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreateRecipeFromImage(
        [FromForm] RecipeRequest recipeRequest,
        IRecipeRequestBackgroundService backgroundService,
        IValidator<RecipeRequest> validator,
        ILogger<RecipeRequest> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating recipe from image.");

        var validationResult = await validator.ValidateAsync(recipeRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.ToDictionary();
            LogCollectionValues(validationErrors.Values, logger);
            return Results.ValidationProblem(validationErrors);
        }
        var image = recipeRequest.Image;
        var memoryStream = new MemoryStream();
        await image!.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var backgroundRecipeRequest = new BackgroundRecipeRequest
        {
            ConnectionId = recipeRequest.ConnectionId,
            UserId = recipeRequest.UserId,
            Image = image,
            Stream = memoryStream
        };

        logger.LogInformation("Enqueuing recipe request with ConnectionId: {connectionId}", recipeRequest.ConnectionId);
        backgroundService.EnqueueRecipeRequest(backgroundRecipeRequest);

        return Results.Ok();
    }

    private static async Task<IResult> GetRecipeFromProcess(
        Guid processId,
        IAiRecipeService recipeService,
        CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetRecipeByProcessId(processId, cancellationToken);
        if (recipe == null)
        {
            return Results.NotFound();
        }

        var recipeResponse = new RecipeResponse
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Ingredients = recipe.Ingredients?.Select(r => new IngredientDto
            {
                Category = r.Category,
                Items = r.Items?.Select(i => new IngredientItemDto
                {
                    Name = i.Name,
                    Calories = i.Calories,
                }).ToList(),
                Calories = r.Calories,
                HealthySubstitutions = r.HealthySubstitutions?.Select(h => new HealthySubstitutionDto
                {
                    Original = h.Original,
                    Substitute = h.Substitute
                }).ToList(),
                CaloriesAfterSubstitution = r.CaloriesAfterSubstitution
            }).ToList(),
            Instructions = recipe.Instructions?.Select(r => new InstructionDto
            {
                Step = r.Step,
                Description = r.Description
            }).ToList(),
            Confidence = recipe.Confidence,
            CookTime = recipe.CookTime,
            Description = recipe.Description,
            Notes = recipe.Notes?.Select(r => new NoteDto
            {
                Text = r.Text
            }).ToList(),
            PrepTime = recipe.PrepTime,
            Rating = recipe.Rating,
            Servings = recipe.Servings,
            TotalTime = recipe.TotalTime,
            Calories = recipe.Calories,
            CaloriesAfterSubstitution = recipe.CaloriesAfterSubstitution
        };

        return Results.Ok(recipeResponse);
    }

    private static async Task<IResult> GetUserRecipes(
         Guid userId,
         IUserService userService,
         IRecipeService recipeService,
         CancellationToken cancellationToken)
    {
        var user = await userService.GetUser(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }
        var recipes = await recipeService.GetRecipes(userId, cancellationToken);
        if (recipes == null)
        {
            return Results.NotFound();
        }
        var recipeResponses = recipes.Select(r => new RecipeResponse
        {
            Id = r.Id,
            Title = r.Title,
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

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        IValidator<RegisterUserRequest> validator,
        IUserService userService,
        ILogger<RegisterUserRequest> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Registering user with email: {email}", request.Email);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var validationErrors = validationResult.ToDictionary();
                LogCollectionValues(validationErrors.Values, logger);
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
            logger.LogError(ex, "Invalid claim exception. Claim type: {claimType}", ex.ClaimType);
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
        Guid id,
        [FromBody] UpdateUserRequest request,
        IUserService userService,
        ILogger<UpdateUserRequest> logger,
        CancellationToken cancellationToken)
    {        
        var user = await userService.GetUser(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.ChefPreference = ChefChoice.FromValue(request.ChefPreference);
        user.Email = request.Email;

        await userService.UpdateUser(user, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetUser(
        Guid id,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUser(u => u.Id == id, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
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

    private static void LogCollectionValues<T>(ICollection<string[]> collection, ILogger<T> logger)
    {
        foreach (var item in collection)
        {
            string concatenatedValues = string.Join(", ", item);
            logger.LogWarning($"Validation errors: {{{LogToken.ValidationErrors}}}", concatenatedValues);
        }
    }
}
