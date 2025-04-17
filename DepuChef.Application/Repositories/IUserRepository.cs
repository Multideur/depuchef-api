using DepuChef.Application.Models.User;
using System.Linq.Expressions;

namespace DepuChef.Application.Repositories;

public interface IUserRepository
{
    Task<User?> Add(User user, CancellationToken cancellationToken);
    Task<User?> GetUser(Expression<Func<User, bool>> expression, CancellationToken cancellationToken);
    Task<User?> GetUser(Guid id, CancellationToken cancellationToken);
    Task Update(User user, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
}
