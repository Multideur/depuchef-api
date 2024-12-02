using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;

namespace DepuChef.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    public Task<User?> Add(User user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUser(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUser(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
