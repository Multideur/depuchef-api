using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Application.Services;

namespace DepuChef.Infrastructure.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            ChefChoice = request.ChefChoice,
            AuthUserId = request.AuthUserId,
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
