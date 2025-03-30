namespace DepuChef.Api.Models;

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int ChefPreference { get; set; }
}
