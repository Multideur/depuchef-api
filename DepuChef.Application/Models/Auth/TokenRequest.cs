namespace DepuChef.Application.Models.Auth;

public class TokenRequest
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Audience { get; set; }
    public string? GrantType { get; set; }
}
