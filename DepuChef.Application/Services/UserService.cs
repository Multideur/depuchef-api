using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Application.Utilities;

namespace DepuChef.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IClaimsHelper claimsHelper
    ) : IUserService
{
    public async Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Email);

        CheckClaims(claimsHelper, out string? authUserId, out string? emailClaim);

        if (emailClaim != request.Email)
        {
            throw new InvalidClaimException("Claim does not match property", ClaimType.Email);
        }

        var existingUser = await GetUser(request.Email, cancellationToken);
        if (existingUser != null)
        {
            if (existingUser.AuthUserId != authUserId)
            {
                throw new InvalidOperationException("User already exists with different AuthUserId");
            }

            return existingUser;
        }

        var user = new User
        {
            ChefPreference = ChefChoice.FromValue(request.ChefPreference, ChefChoice.Aduke),
            AuthUserId = authUserId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            SubscriptionLevel = SubscriptionLevel.FromValue(request.Subscription, SubscriptionLevel.Free)
        };

        return await userRepository.Add(user, cancellationToken);
    }

    public async Task<User?> GetUser(string email, CancellationToken cancellationToken)
    {
        CheckClaims(claimsHelper, out string? authUserId, out _);
        var user = await userRepository.GetUser(email, cancellationToken);

        return user; ;
    }

    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken)
    {
        CheckClaims(claimsHelper, out string? authUserId, out _);
        var user = await userRepository.GetUser(id, cancellationToken);

        return user?.AuthUserId != authUserId 
            ? throw new InvalidOperationException("User not found or mismatched.") 
            : user;
    }

    private static void CheckClaims(IClaimsHelper claimsHelper, out string? authUserId, out string? emailClaim)
    {
        var claims = claimsHelper.RetrieveClaims() ?? throw new Exception("Claims are required.");
        authUserId = claims.SingleOrDefault(claim => claim.Type == ClaimType.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(authUserId))
            throw new InvalidClaimException(ClaimType.Sub);
        emailClaim = claims.SingleOrDefault(claim => claim.Type == ClaimType.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaim))
            throw new InvalidClaimException(ClaimType.Email);
    }
}
