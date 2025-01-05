namespace DepuChef.Application.Exceptions;

public class InvalidClaimException : Exception
{

    public InvalidClaimException(string claimType) : base($"Invalid claim: {claimType}")
    {
        ClaimType = claimType;
    }

    public InvalidClaimException(string message, string claimType) : base(message)
    {
        ClaimType = claimType;
    }

    public string ClaimType { get; set; }
}
