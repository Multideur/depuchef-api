using DepuChef.Application.Models.User;

namespace DepuChef.Application.Repositories;

public interface IUserRepository
{
    Task<User?> Add(User user, CancellationToken cancellationToken);
    Task<User?> GetUser(string email, CancellationToken cancellationToken);
    Task<User?> GetUser(Guid id, CancellationToken cancellationToken);
}
