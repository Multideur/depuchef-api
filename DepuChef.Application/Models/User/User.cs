using DepuChef.Application.Constants;

namespace DepuChef.Application.Models.User;

public class User
{
    public Guid Id { get; set; }
    public string? AuthUserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public SubscriptionLevel? SubscriptionLevel { get; set; }
    public ChefChoice? ChefChoice { get; set; }
}
