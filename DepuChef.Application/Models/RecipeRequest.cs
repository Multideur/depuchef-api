using Microsoft.AspNetCore.Http;

namespace DepuChef.Application.Models;

public class RecipeRequest
{
    public string? ConnectionId { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? Image { get; set; }
}
