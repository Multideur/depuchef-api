using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Application.Services;
using DepuChef.Application.Utilities;
using FluentAssertions;
using Moq;
using System.Security.Claims;

namespace DepuChef.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IClaimsHelper> _mockClaimsHelper = new();

    [Fact]
    public async Task GetUserById_WhenClaimsAreInvalid_Throw()
    {
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        var user = new User
        {
            Id = userId,
            ChefPreference = ChefChoice.Aduke,
            SubscriptionLevel = SubscriptionLevel.Free
        };

        _mockUserRepository.Setup(x => x.GetUser(userId, default)).ReturnsAsync(user);

        var act = () => sut.GetUser(u => u.Id == userId, default);

        await act.Should().ThrowAsync<InvalidClaimException>();
    }

    [Fact]
    public async Task GetUserById_WhenClaimsAreValid_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sut = CreateSut();
        var email = "test@test.com";
        var user = new User
        {
            Id = userId,
            ChefPreference = ChefChoice.Aduke,
            SubscriptionLevel = SubscriptionLevel.Free
        };
        _mockUserRepository.Setup(x => x.GetUser(u => u.Id == userId, default)).ReturnsAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, userId.ToString())
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        var result = await sut.GetUser(u => u.Id == userId, default);

        result.Should().Be(user);
    }

    [Fact]
    public async Task RegisterUser_WhenClaimsAreInvalid_Throw()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var request = new RegisterUserRequest
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Subscription = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var act = () => sut.RegisterUser(request, default);

        await act.Should().ThrowAsync<InvalidClaimException>();
    }

    [Fact]
    public async Task RegisterUser_WhenNewUserAdded_ReturnsUser()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var request = new RegisterUserRequest
        {
            Email = email,
            FirstName = "Test",
            LastName = "User",
            Subscription = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, userId.ToString())
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        var user = new User
        {
            Id = userId,
            ChefPreference = ChefChoice.Aduke,
            SubscriptionLevel = SubscriptionLevel.Free
        };

        _mockUserRepository.Setup(x => x.GetUser(u => u.Email == email, default)).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.Add(It.IsAny<User>(), default)).ReturnsAsync(user);

        // Act
        var result = await sut.RegisterUser(request, default);

        // Assert
        result.Should().Be(user);
    }

    [Fact]
    public async Task RegisterUser_WhenUserAlreadyExistsWithDifferentId_ReturnsExistingUser()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var request = new RegisterUserRequest
        {
            Email = email,
            FirstName = "Test",
            LastName = "User",
            Subscription = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, "authUserId")
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        var user = new User
        {
            Id = userId,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            ChefPreference = ChefChoice.Aduke,
            SubscriptionLevel = SubscriptionLevel.Free
        };

        _mockUserRepository.Setup(x => x.GetUser(u => u.Email == request.Email, default)).ReturnsAsync(user);

        var act = async() => await sut.RegisterUser(request, default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RegisterUser_WhenUserAlreadyExistsWithSameId_ReturnsExistingUser()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var authUserId = "test auth id";
        var email = "test@test.com";
        var request = new RegisterUserRequest
        {
            Email = email,
            FirstName = "Test",
            LastName = "User",
            Subscription = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, authUserId)
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            ChefPreference = ChefChoice.Aduke,
            SubscriptionLevel = SubscriptionLevel.Free
        };

        _mockUserRepository.Setup(x => x.GetUser(u => u.Email == request.Email, default)).ReturnsAsync(user);

        var result = await sut.RegisterUser(request, default);

        result.Should().Be(user);
    }

    [Fact]
    public async Task UpdateUser_WhenUserIdIsDifferent_Throw()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var authUserId = "test auth id";
        var email = "test@test.com";
        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            FirstName = "Test",
            LastName = "User",
            SubscriptionLevel = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, "different auth id")
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        var act = () => sut.UpdateUser(user, default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenAuthIdIsSame_UpdateUser()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var authUserId = "test auth id";
        var email = "test@test.com";
        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            FirstName = "Test",
            LastName = "User",
            SubscriptionLevel = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Aduke
        };

        var claims = new List<Claim>
        {
            new(ClaimType.Email, email),
            new(ClaimType.Sub, authUserId)
        };
        _mockClaimsHelper.Setup(x => x.RetrieveClaims(null)).Returns(claims);

        await sut.UpdateUser(user, default);

        _mockUserRepository.Verify(x => x.Update(user, default), Times.Once);
    }

    private UserService CreateSut() => new(_mockUserRepository.Object, _mockClaimsHelper.Object);
}
