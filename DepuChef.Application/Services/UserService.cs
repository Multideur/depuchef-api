using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Application.Utilities;
using System.Linq.Expressions;

namespace DepuChef.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IAdminUserRepository adminUserRepository,
    IAuthManagementService authManagementService,
    IClaimsHelper claimsHelper
    ) : IUserService
{
    public async Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Email);

        claimsHelper.CheckClaims(out string? authUserId, out string? emailClaim);

        if (emailClaim != request.Email)
        {
            throw new InvalidClaimException("Claim does not match property", ClaimType.Email);
        }

        var existingUser = await GetUser(user => user.Email == request.Email, cancellationToken);
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
            ChefPreference = ChefChoice.FromValue(request.ChefPreference, ChefChoice.Femi),
            AuthUserId = authUserId,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            VirtualCoins = 150,
            SubscriptionLevel = SubscriptionLevel.FromValue(request.Subscription, SubscriptionLevel.Free)
        };

        return await userRepository.Add(user, cancellationToken);
    }

    public async Task<User?> GetUser(Expression<Func<User, bool>> expression, CancellationToken cancellationToken)
    {
        claimsHelper.CheckClaims(out _, out _);
        var user = await userRepository.GetUser(expression, cancellationToken);

        return user; ;
    }

    public async Task UpdateUser(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        claimsHelper.CheckClaims(out string? authUserId, out _);

        var userIsAdmin = await IsAdmin(cancellationToken);
        if (!userIsAdmin && user.AuthUserId != authUserId)
        {
            throw new InvalidOperationException("User does not have permission to update this user");
        }

        await userRepository.Update(user, cancellationToken);
    }

    public async Task ArchiveUser(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        claimsHelper.CheckClaims(out string? authUserId, out _);

        var userIsAdmin = await IsAdmin(cancellationToken);
        if (!userIsAdmin && user.AuthUserId != authUserId)
        {
            throw new InvalidOperationException("User does not have permission to archive this user");
        }

        user.IsArchived = true;
        user.ArchivedAt = DateTime.UtcNow;
        user.ArchivedBy = authUserId;
        await userRepository.Update(user, cancellationToken);
    }

    public async Task DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        claimsHelper.CheckClaims(out string? authUserId, out _);
        var userIsAdmin = await IsAdmin(cancellationToken);
        var user = await userRepository.GetUser(u => u.Id == id, cancellationToken) ?? 
            throw new InvalidOperationException("User not found");

        if (!userIsAdmin && user.AuthUserId != authUserId)
        {
            throw new InvalidOperationException("User does not have permission to delete this user");
        }

        if (user.AuthUserId == null)
        {
            throw new InvalidOperationException("User does not have AuthUserId");
        }

        await userRepository.Delete(id, cancellationToken);
        await authManagementService.DeleteUser(user.AuthUserId, cancellationToken);
    }

    public async Task<bool> IsAdmin(CancellationToken cancellationToken)
    {
        claimsHelper.CheckClaims(out _, out string? emailClaim);
        var user = await adminUserRepository.Get(a => a.Email == emailClaim, cancellationToken);
        return user != null;
    }
}
