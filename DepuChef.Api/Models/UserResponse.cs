using DepuChef.Application.Constants;

namespace DepuChef.Api.Models;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public SubscriptionLevel? SubscriptionLevel { get; set; }
    public ChefChoice? ChefPreference { get; set; }
}
