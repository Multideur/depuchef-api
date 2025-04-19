namespace DepuChef.Application.Models.Auth;

public class AuthenticationOptions
{
    public const string Options = "Authentication";
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Audience { get; set; }
}
