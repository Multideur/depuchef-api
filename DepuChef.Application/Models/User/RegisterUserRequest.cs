using DepuChef.Application.Constants;

namespace DepuChef.Application.Models.User;

public class RegisterUserRequest
{
    public string? Email { get; set; }
    public string? FirstName  { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int ChefPreference { get; set; } = ChefChoice.Aduke.Value;
    public int Subscription { get; set; } = SubscriptionLevel.Free.Value;
}
