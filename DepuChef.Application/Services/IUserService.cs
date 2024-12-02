using DepuChef.Application.Models.User;

namespace DepuChef.Application.Services;

public interface IUserService
{
    Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<User?> GetUser(string email, CancellationToken cancellationToken);
    Task<User?> GetUser(Guid id, CancellationToken cancellationToken);
}
