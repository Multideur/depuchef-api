using DepuChef.Application.Models;
using DepuChef.Application.Repositories;
using DepuChef.Application.Services;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;

namespace DepuChef.Application.Tests.Services;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository = new();

    [Fact]
    public async Task GetRecipesForUser_WhenRecipeExists_ReturnsRecipe()
    {
        var recipeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        var recipe = new Recipe
        {
            Id = recipeId,
            UserId = userId,
            Title = "Test Recipe",
            Ingredients =
            [
                new Ingredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    Calories = 100,
                    Items = 
                    [
                        new IngredientItem
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Ingredient"
                        }
                    ]
                }
            ]
        };
        _mockRecipeRepository.Setup(x => x.GetRecipes(r => r.UserId == userId, default)).ReturnsAsync([recipe]);

        var result = await sut.GetRecipesForUser(userId, default);

        result.Should().Contain(recipe);
    }

    [Fact]
    public async Task GetRecipesForUser_WhenRecipeDoesNotExist_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        _mockRecipeRepository.Setup(x => x.GetRecipes(r => r.UserId == userId, default)).ReturnsAsync([]);

        var result = await sut.GetRecipesForUser(userId, default);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateRecipeFavourite_WhenRecipeDoesNotExist_ReturnsNull()
    {
        var recipeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        _mockRecipeRepository.Setup(x => x.GetRecipe(recipeId, default)).ReturnsAsync((Recipe?)null);

        var result = await sut.UpdateRecipeFavourite(userId, recipeId, true, default);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRecipeFavourite_WhenRecipeExists_ReturnsUpdatedRecipe()
    {
        var recipeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        var recipe = new Recipe
        {
            IsFavourite = false,
            Id = recipeId,
            UserId = userId,
            Title = "Test Recipe",
            Ingredients =
            [
                new Ingredient
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    Calories = 100,
                    Items =
                    [
                        new IngredientItem
                        {
                            Id = Guid.NewGuid(),
                            Name = "Test Ingredient"
                        }
                    ]
                }
            ]
        };
        _mockRecipeRepository.Setup(x => x.GetRecipe(recipeId, default)).ReturnsAsync(recipe);
        _mockRecipeRepository.Setup(x => x.Update(It.Is<Recipe>(r => r.Id == recipeId), default)).ReturnsAsync(recipe);

        var result = await sut.UpdateRecipeFavourite(userId, recipeId, true, default);

        using var _ = new AssertionScope();
        result.Should().Be(recipe);
        result.IsFavourite.Should().BeTrue();
    }

    private RecipeService CreateSut() => new(_mockRecipeRepository.Object);
}
