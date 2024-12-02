using Microsoft.AspNetCore.Http;

namespace DepuChef.Application.Models;

public class RecipeRequest
{
    public string? ConnectionId { get; set; }
    public IFormFile? Image { get; set; }
}
