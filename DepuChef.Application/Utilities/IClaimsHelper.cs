using System.Security.Claims;

namespace DepuChef.Application.Utilities;

public interface IClaimsHelper
{
    IEnumerable<Claim> RetrieveClaimsFromToken(string? token = null);
    void CheckClaims(out string? authUserId, out string? emailClaim);
}
