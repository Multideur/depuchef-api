using DepuChef.Application.Models.User;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace DepuChef.Application.Services;

public interface IUserService
{
    Task<User?> RegisterUser(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<User?> GetUser(Expression<Func<User, bool>> expression, CancellationToken cancellationToken);
    Task UpdateUser(User user, CancellationToken cancellationToken);
    Task UpdateUserProfilePicture(Guid userId, IFormFile file, CancellationToken cancellationToken);
    Task ArchiveUser(User user, CancellationToken cancellationToken);
    Task DeleteUser(Guid id, CancellationToken cancellationToken);
    Task<bool> IsAdmin(CancellationToken cancellationToken);
}
