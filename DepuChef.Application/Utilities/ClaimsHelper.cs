using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DepuChef.Application.Utilities;

public class ClaimsHelper(IHttpContextAccessor httpContextAccessor) : IClaimsHelper
{
    public IEnumerable<Claim> RetrieveClaims(string? token = null)
    {
        if (token == null)
        {
            var httpContext = httpContextAccessor?.HttpContext;
            token = httpContext?.Request?.Headers.Authorization;
        }

        if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
        {
            token = token["Bearer ".Length..];

            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(token) is JwtSecurityToken jsonToken)
            {
                return jsonToken.Claims;
            }
        }

        return [];
    }
}
