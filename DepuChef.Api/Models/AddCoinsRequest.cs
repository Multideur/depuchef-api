namespace DepuChef.Api.Models;

public class AddCoinsRequest
{
    public Guid UserId { get; set; }
    public int Coins { get; set; }
}
