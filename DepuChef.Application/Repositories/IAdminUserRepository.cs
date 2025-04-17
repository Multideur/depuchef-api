using DepuChef.Application.Models.User;
using System.Linq.Expressions;

namespace DepuChef.Application.Repositories;

public interface IAdminUserRepository
{
    Task<AdminUser?> Get(Expression<Func<AdminUser, bool>> predicate, CancellationToken cancellationToken);
}
