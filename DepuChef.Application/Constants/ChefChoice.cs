using Ardalis.SmartEnum;

namespace DepuChef.Application.Constants;

public sealed class ChefChoice : SmartEnum<ChefChoice>
{
    public static readonly ChefChoice Aduke = new(nameof(Aduke), 1);
    public static readonly ChefChoice Michael = new(nameof(Michael), 2);

    private ChefChoice(string name, int value) : base(name, value)
    {
    }
}
