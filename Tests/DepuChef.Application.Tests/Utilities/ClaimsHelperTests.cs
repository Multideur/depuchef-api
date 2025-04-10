using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Utilities;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DepuChef.Application.Tests.Utilities;

public class ClaimsHelperTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

    [Fact]
    public void RetrieveClaimsFromToken_ValidToken_ReturnsClaims()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Sub, "123"), 
            new Claim(ClaimType.Email, "test@example.com")
            ]);
        var sut = CreateSut();

        var claims = sut.RetrieveClaimsFromToken($"Bearer {token}");

        using var _ = new AssertionScope();
        claims.Should().NotBeNullOrEmpty();
        claims.Should().HaveCount(2);
        claims.Should().Contain(c => c.Type == ClaimType.Sub && c.Value == "123");
        claims.Should().Contain(c => c.Type == ClaimType.Email && c.Value == "test@example.com");
    }

    [Fact]
    public void RetrieveClaimsFromToken_NoBearerScheme_ReturnsEmpty()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Sub, "123"),
            new Claim(ClaimType.Email, "test@example.com")
            ]);
        var sut = CreateSut();

        var claims = sut.RetrieveClaimsFromToken(token);

        claims.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Invalid Token")]
    public void RetrieveClaimsFromToken_InvalidToken_ReturnsEmpty(string? token)
    {
        var sut = CreateSut();

        var claims = sut.RetrieveClaimsFromToken(token);

        claims.Should().BeEmpty();
    }

    [Fact]
    public void RetrieveClaimsFromToken_NoAuthorizationHeader_ReturnsEmpty()
    {
        var sut = CreateSut();

        var claims = sut.RetrieveClaimsFromToken();

        claims.Should().BeEmpty();
    }

    [Fact]
    public void RetrieveClaimsFromToken_ValidAuthorizationHeader_ReturnsClaims()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Sub, "123"),
            new Claim(ClaimType.Email, "test@example.com")
            ]);
        SetupHttpContextWithToken(token);
        var sut = CreateSut();
        var claims = sut.RetrieveClaimsFromToken();

        using var _ = new AssertionScope();
        claims.Should().NotBeNullOrEmpty();
        claims.Should().HaveCount(2);
        claims.Should().Contain(c => c.Type == ClaimType.Sub && c.Value == "123");
        claims.Should().Contain(c => c.Type == ClaimType.Email && c.Value == "test@example.com");
    }

    [Fact]
    public void CheckClaims_ValidClaims_SetsOutParameters()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Sub, "123"),
            new Claim(ClaimType.Email, "test@example.com")
            ]);
        SetupHttpContextWithToken(token);
        var sut = CreateSut();

        sut.CheckClaims(out var authUserId, out var emailClaim);

        using var _ = new AssertionScope();
        authUserId.Should().Be("123");
        emailClaim.Should().Be("test@example.com");
    }

    [Fact]
    public void CheckClaims_MissingSubClaim_ThrowsInvalidClaimException()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Email, "test@example.com")
            ]);
        SetupHttpContextWithToken(token);
        var sut = CreateSut();

        var act = () => sut.CheckClaims(out _, out _);

        act.Should().Throw<InvalidClaimException>();
    }

    [Fact]
    public void CheckClaims_MissingEmailClaim_ThrowsInvalidClaimException()
    {
        var token = GenerateJwtToken([
            new Claim(ClaimType.Sub, "123")
            ]);
        SetupHttpContextWithToken(token);
        var sut = CreateSut();

        var act = () => sut.CheckClaims(out _, out _);

        act.Should().Throw<InvalidClaimException>();
    }

    private static string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = new JwtSecurityToken(claims: claims);
        return tokenHandler.WriteToken(token);
    }

    private void SetupHttpContextWithToken(string token)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = $"Bearer {token}";
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
    }

    private ClaimsHelper CreateSut()
    {
        return new ClaimsHelper(
            _mockHttpContextAccessor.Object
            );
    }
}
