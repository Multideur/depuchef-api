using DepuChef.Application.Models.User;
using DepuChef.Application.Repositories;
using DepuChef.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DepuChef.Infrastructure.Repositories;

public class UserRepositoryBkup(DepuChefDbContext databaseContext) : IUserRepository
{
    public async Task<User?> Add(User user, CancellationToken cancellationToken)
    {
        var entityEntry = await databaseContext.Users.AddAsync(user, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity;
    }

    public async Task<User?> GetUser(string email, CancellationToken cancellationToken) => 
        await databaseContext.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken) => 
        await databaseContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
}
