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
        var claims = claimsHelper.RetrieveClaims() ?? throw new Exception("Claims are required.");
        var authUserId = claims.Single(claim => claim.Type == ClaimType.Sub).Value;
        if (string.IsNullOrWhiteSpace(authUserId))
            throw new InvalidClaimException(ClaimType.Sub);
        var emailClaim = claims.SingleOrDefault(claim => claim.Type == ClaimType.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaim))
            throw new InvalidClaimException(ClaimType.Email);

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
            ChefPreference = request.ChefPreference,
            AuthUserId = authUserId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            SubscriptionLevel = request.SubscriptionLevel
        };

        return await userRepository.Add(user, cancellationToken);
    }

    public async Task<User?> GetUser(string email, CancellationToken cancellationToken) => await userRepository.GetUser(email, cancellationToken);

    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken) => await userRepository.GetUser(id, cancellationToken);
}
