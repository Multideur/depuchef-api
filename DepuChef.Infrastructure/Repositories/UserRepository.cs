using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DepuChef.Infrastructure.Repositories;

public class UserRepository(DepuChefDbContext databaseContext) : IUserRepository
{
    public async Task<User?> Add(User user, CancellationToken cancellationToken)
    {
        var entityEntry = await databaseContext.Users.AddAsync(user, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity;
    }

    public async Task<User?> GetUser(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken) => 
        await databaseContext.Users.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken) => 
        await databaseContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    public async Task Update(User user, CancellationToken cancellationToken)
    {
        databaseContext.Users.Update(user);
        await databaseContext.SaveChangesAsync(cancellationToken);
    }
}
