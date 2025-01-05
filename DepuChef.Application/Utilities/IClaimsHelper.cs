using System.Security.Claims;

namespace DepuChef.Application.Utilities;

public interface IClaimsHelper
{
    IEnumerable<Claim> RetrieveClaims(string? token = null);
}
