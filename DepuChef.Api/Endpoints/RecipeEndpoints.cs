using DepuChef.Api.Models;
using DepuChef.Api.Utilities;
using DepuChef.Application.Constants;
using DepuChef.Application.Models;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DepuChef.Api.Endpoints;

public static class RecipeEndpoints
{
    public static void MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var recipeRoute = app.MapGroup("/recipe");
        recipeRoute.MapPost("/create", CreateRecipe)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("GenerateRecipe")
            .WithOpenApi();

        recipeRoute.MapGet("/{processId}", GetRecipeFromProcess)
            .RequireAuthorization()
            .Produces<RecipeResponse>();
    }

    private static async Task<IResult> CreateRecipe(
        [FromForm] RecipeRequest recipeRequest,
        IRecipeRequestBackgroundService backgroundService,
        IUserService userService,
        IValidator<RecipeRequest> validator,
        ILogger<RecipeRequest> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating recipe from image or text.");

        var validationResult = await validator.ValidateAsync(recipeRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.ToDictionary();
            LoggingHelper.LogCollectionValues(validationErrors.Values, logger);
            return Results.ValidationProblem(validationErrors);
        }

        var user = await userService.GetUser(u => !u.IsArchived && u.Id == recipeRequest.UserId, cancellationToken);

        if (user == null)
        {
            logger.LogWarning($"User not found with Id: {{{LogToken.UserId}}}", recipeRequest.UserId);
            var error = new ProblemDetails
            {
                Title = "User not found.",
                Detail = "User not found."
            };
            return Results.NotFound(error);
        }

        if (user.VirtualCoins < 5)
        {
            logger.LogWarning($"Insufficient coins for user with Id: {{{LogToken.UserId}}}", recipeRequest.UserId);
            var error = new ProblemDetails
            {
                Title = "Insufficient coins.",
                Detail = "Insufficient coins."
            };
            return Results.BadRequest(error);
        }

        var image = recipeRequest.Image;
        var memoryStream = new MemoryStream();
        if (image != null)
        {
            await image!.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
        }

        var backgroundRecipeRequest = new BackgroundRecipeRequest
        {
            ConnectionId = recipeRequest.ConnectionId,
            UserId = recipeRequest.UserId,
            Image = image,
            Stream = memoryStream,
            Text = recipeRequest.Text,
        };

        logger.LogInformation($"Enqueuing recipe request with ConnectionId: {{{LogToken.ConnectionId}}}", recipeRequest.ConnectionId);
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
}