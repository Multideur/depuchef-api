using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepuChef.Infrastructure.Repositories;

public class AdminUserRepository(DepuChefDbContext dbContext) : IAdminUserRepository
{
    public async Task<AdminUser?> Get(Expression<Func<AdminUser, bool>> predicate, CancellationToken cancellationToken) => 
        await dbContext.AdminUsers.FirstOrDefaultAsync(predicate, cancellationToken);
}
