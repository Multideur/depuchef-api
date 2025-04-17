using DepuChef.Application.Constants;

namespace DepuChef.Application.Models.User;

public class User : AuditedModel
{
    public Guid Id { get; set; }
    public string? AuthUserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int VirtualCoins { get; set; }
    public ICollection<Recipe>? Recipes { get; set; }
    public required SubscriptionLevel SubscriptionLevel { get; set; }
    public required ChefChoice ChefPreference { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedBy { get; set; }
}
