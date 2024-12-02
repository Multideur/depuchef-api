using DepuChef.Application.Constants;

namespace DepuChef.Application.Models.User;

public class RegisterUserRequest
{
    public string? Email { get; set; }
    public string? AuthUserId { get; set; }
    public string? FirstName  { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public ChefChoice? ChefChoice { get; set; } = ChefChoice.Female;
    public SubscriptionLevel SubscriptionLevel { get; set; } = SubscriptionLevel.Free;
}
