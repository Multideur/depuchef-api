namespace DepuChef.Application.Services;

public interface IAuthManagementService
{
    Task DeleteUser(string authUserId, CancellationToken cancellationToken);
}
