﻿using DepuChef.Application.Models;
using DepuChef.Application.Services;
using DepuChef.Application.Services.OpenAi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Channels;

namespace DepuChef.Application.Tests.Services;

public class RecipeRequestBackgroundServiceTests
{
    private const int MillisecondsDelay = 500;
    private readonly Mock<IAiRecipeService> _mockRecipeService = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<BackgroundRecipeRequest> _requestChannel = Channel.CreateUnbounded<BackgroundRecipeRequest>();
    private readonly Mock<ILogger<RecipeRequestBackgroundService>> _mockLogger = new();

    public RecipeRequestBackgroundServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_ => _mockRecipeService.Object);

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task EnqueueRecipeRequestWithImage_ShouldCreateRecipe()
    {
        var recipeRequest = new BackgroundRecipeRequest
        {
            Image = new FormFile(Stream.Null, 0, 0, "fileName", "fileName"),
            Stream = new MemoryStream(),
            ConnectionId = "connectionId"
        };
        var sut = CreateSut();
        var cancellationTokenSource = new CancellationTokenSource();

        sut.EnqueueRecipeRequest(recipeRequest);
        _ = sut.StartChannelListener(cancellationTokenSource.Token);

        await Task.Delay(MillisecondsDelay);

        cancellationTokenSource.Cancel();

        _mockRecipeService.Verify(x => x.CreateRecipeFromImage(recipeRequest, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task EnqueueRecipeRequestWithText_ShouldCreateRecipe()
    {
        var recipeRequest = new BackgroundRecipeRequest
        {
            Text = "text",
            ConnectionId = "connectionId"
        };
        var sut = CreateSut();
        var cancellationTokenSource = new CancellationTokenSource();

        sut.EnqueueRecipeRequest(recipeRequest);
        _ = sut.StartChannelListener(cancellationTokenSource.Token);
        await Task.Delay(MillisecondsDelay);
        cancellationTokenSource.Cancel();

        _mockRecipeService.Verify(x => x.CreateRecipeFromText(recipeRequest, cancellationTokenSource.Token));
    }

    private RecipeRequestBackgroundService CreateSut() =>
        new(
            _serviceProvider,
            _requestChannel,
            _mockLogger.Object
            );
}