using Microsoft.AspNetCore.Http;

namespace DepuChef.Application.Models.OpenAI.Thread;

public class RunRequest
{
    public string? ThreadId { get; set; }
    public IFormFile? File { get; set; }
}