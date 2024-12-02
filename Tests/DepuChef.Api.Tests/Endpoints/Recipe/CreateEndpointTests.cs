using DepuChef.Api.FunctionalTests.TestInfrastructure;
using FluentAssertions;
using System.Net;

namespace DepuChef.Api.FunctionalTests.Endpoints.Recipe;

[Collection("Functional Tests")]
public class CreateEndpointTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    private const string RecipeCreateEndpoint = "/api/v1/recipe/create";

    [Fact]
    public async Task CreateRecipeFromImage_ShouldReturnBadRequest_WhenImageIsNull()
    {
        // Arrange
        var client = new TestApiApplication().CreateClient();
        var request = new MultipartFormDataContent();
        var response = await client.PostAsync(RecipeCreateEndpoint, request);
        // Act
        var content = await response.Content.ReadAsStringAsync();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Be("Image is required.");
    }
}
