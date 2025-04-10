using DepuChef.Application.Constants;
using DepuChef.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DepuChef.Application.Utilities;

public class ClaimsHelper(IHttpContextAccessor httpContextAccessor) : IClaimsHelper
{
    public IEnumerable<Claim> RetrieveClaimsFromToken(string? token = null)
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

    public void CheckClaims(out string? authUserId, out string? emailClaim)
    {
        var claims = RetrieveClaimsFromToken() ?? throw new Exception("Claims are required.");
        authUserId = claims.SingleOrDefault(claim => claim.Type == ClaimType.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(authUserId))
            throw new InvalidClaimException(ClaimType.Sub);
        emailClaim = claims.SingleOrDefault(claim => claim.Type == ClaimType.Email)?.Value;
        if (string.IsNullOrWhiteSpace(emailClaim))
            throw new InvalidClaimException(ClaimType.Email);
    }
}
