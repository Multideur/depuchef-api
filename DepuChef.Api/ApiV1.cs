﻿using DepuChef.Api.Models;
using DepuChef.Application.Models;
using DepuChef.Application.Models.User;
using DepuChef.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DepuChef.Api;

public static class ApiV1
{
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapPost("/recipe/create", CreateRecipeFromImage)
            .RequireAuthorization()
            .DisableAntiforgery()
            .WithName("GenerateRecipe")
            .WithOpenApi();

        app.MapGet("/recipe/{threadId}", GetRecipeFromThread)
            .RequireAuthorization();

        app.MapPost("/identity/register", RegisterUser)
            .RequireAuthorization();

        app.MapGet("/test", () =>
        {
            Console.WriteLine("This is a test log. Depuchef");
            return Results.Ok("Hello, World!");
        }).RequireAuthorization();
    }

    private static async Task<IResult> CreateRecipeFromImage(
        [FromForm] RecipeRequest recipeRequest,
        [FromServices] IRecipeRequestBackgroundService backgroundService,
        ILogger<RecipeRequest> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating recipe from image.");
        var image = recipeRequest.Image;
        if (image == null)
        {
            return Results.BadRequest("Image is required.");
        }

        if (image.Length == 0)
        {
            return Results.BadRequest("Image is empty.");
        }

        if (image.Length > 5_000_000)
        {
            return Results.BadRequest("Image is too large.");
        }

        if (!image.ContentType.Contains("image"))
        {
            return Results.BadRequest("File is not an image.");
        }

        var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var backgroundRecipeRequest = new BackgroundRecipeRequest
        {
            ConnectionId = recipeRequest.ConnectionId,
            Image = image,
            Stream = memoryStream
        };

        logger.LogInformation("Enqueuing recipe request with ConnectionId: {connectionId}", recipeRequest.ConnectionId);
        backgroundService.EnqueueRecipeRequest(backgroundRecipeRequest);

        return Results.Ok();
    }

    private static async Task<IResult> GetRecipeFromThread(
        string threadId,
        IRecipeService recipeService,
        CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetRecipeFromThread(threadId, cancellationToken);
        if (recipe == null)
        {
            return Results.NotFound();
        }

        var recipeResponse = new RecipeResponse
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Ingredients = recipe.Ingredients.Select(r => new IngredientDto
            {
                Category = r.Category,
                Items = r.Items
            }).ToList(),
            Instructions = recipe.Instructions.Select(r => new InstructionDto
            {
                Step = r.Step,
                Description = r.Description
            }).ToList(),
            Confidence = recipe.Confidence,
            CookTime = recipe.CookTime,
            Description = recipe.Description,
            Notes = recipe.Notes.Select(r => new NoteDto
            {
                Text = r.Text
            }).ToList(),
            PrepTime = recipe.PrepTime,
            Rating = recipe.Rating,
            Servings = recipe.Servings,
            TotalTime = recipe.TotalTime
        };

        return Results.Ok(recipeResponse);
    }

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserRequest registerUserRequest,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.RegisterUser(registerUserRequest, cancellationToken);
        if (result == null)
        {
            return Results.BadRequest();
        }

        return Results.Ok(result);
    }
}
