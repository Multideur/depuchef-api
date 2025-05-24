using DepuChef.Application.Constants;
using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Application.Services;
using DepuChef.Application.Utilities;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace DepuChef.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IAdminUserRepository> _mockAdminUserRepository = new();
    private readonly Mock<IAuthManagementService> _mockAuthManagementService = new();
    private readonly Mock<IClaimsHelper> _mockClaimsHelper = new();
    private readonly Mock<IStorageService> _mockStorageService = new();
    private readonly Mock<ILogger<UserService>> _mockLogger = new();

    [Fact]
    public async Task GetUser_WhenClaimsAreValid_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var subClaim = "test auth id";
        var sut = CreateSut();
        var email = "test@test.com";
        var user = new User
        {
            Id = userId,
            ChefPreference = ChefChoice.Femi,
            SubscriptionLevel = SubscriptionLevel.Free
        };
        _mockUserRepository.Setup(x => x.GetUser(u => u.Id == userId, default)).ReturnsAsync(user);
        _mockClaimsHelper.Setup(x => x.CheckClaims(out subClaim, out email));

        var result = await sut.GetUser(u => u.Id == userId, default);

        result.Should().Be(user);
        _mockClaimsHelper.Verify(x => x.CheckClaims(out subClaim, out email), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WhenNewUserAdded_ReturnsUser()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var subClaim = "test auth id";
        var email = "test@test.com";
        var request = new RegisterUserRequest
        {
            Email = email,
            FirstName = "Test",
            LastName = "User",
            Subscription = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Femi
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out subClaim, out email));

        var user = new User
        {
            Id = userId,
            ChefPreference = ChefChoice.Femi,
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
    public async Task RegisterUser_WhenUserAlreadyExists_Throw()
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
            ChefPreference = ChefChoice.Femi
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out authUserId, out email));

        var user = new User
        {
            Id = userId,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            ChefPreference = ChefChoice.Femi,
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
            ChefPreference = ChefChoice.Femi
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out authUserId, out email));

        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            ChefPreference = ChefChoice.Femi,
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
        var differentAuthUserId = "test auth id different";
        var email = "test@test.com";
        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            FirstName = "Test",
            LastName = "User",
            SubscriptionLevel = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Femi
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out differentAuthUserId, out email));

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
            ChefPreference = ChefChoice.Femi
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out authUserId, out email));

        await sut.UpdateUser(user, default);

        _mockUserRepository.Verify(x => x.Update(user, default), Times.Once);
    }

    [Fact]
    public async Task UpdateUserProfilePicture_WhenAuthIdIsSame_UpdateUser()
    {
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var authUserId = "test auth id";
        var email = "test@test.com";
        var file = new Mock<IFormFile>();
        var user = new User
        {
            Id = userId,
            AuthUserId = authUserId,
            FirstName = "Test",
            LastName = "User",
            SubscriptionLevel = SubscriptionLevel.Free,
            ChefPreference = ChefChoice.Femi
        };
        var fileName = "test.jpg";
        var fileNameExtension = Path.GetExtension(fileName);
        var fileNameWithId = "images/user-profile/" + userId + $"/{Guid.NewGuid()}{fileNameExtension}";
        file.Setup(x => x.FileName).Returns(fileName);
        _mockClaimsHelper.Setup(x => x.CheckClaims(out authUserId, out email));
        _mockUserRepository.Setup(x => x.GetUser(u => u.Id == userId, default)).ReturnsAsync(user);
        _mockStorageService.Setup(x => x.UploadFile(file.Object, It.Is<string>(s => s.Contains(userId.ToString())), default)).ReturnsAsync(fileNameWithId);

        await sut.UpdateUserProfilePicture(userId, file.Object, default);

        _mockUserRepository.Verify(x => x.Update(user, default), Times.Once);
    }

    [Fact]
    public async Task ArchiveUser_WhenUserFound_Archives()
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
            ChefPreference = ChefChoice.Femi,
            IsArchived = false
        };

        _mockClaimsHelper.Setup(x => x.CheckClaims(out authUserId, out email));
        _mockUserRepository.Setup(x => x.GetUser(u => u.Id == userId, default)).ReturnsAsync(user);

        await sut.ArchiveUser(user, default);

        _mockUserRepository.Verify(x => x.Update(user, default), Times.Once);
        using var _ = new AssertionScope();
        user.IsArchived.Should().BeTrue();
        user.ArchivedAt.Should().NotBeNull();
        user.ArchivedBy.Should().Be(authUserId);
    }

    private UserService CreateSut() => 
        new(
            _mockUserRepository.Object, 
            _mockAdminUserRepository.Object,
            _mockAuthManagementService.Object,
            _mockClaimsHelper.Object,
            _mockStorageService.Object,
            _mockLogger.Object
            );
}
