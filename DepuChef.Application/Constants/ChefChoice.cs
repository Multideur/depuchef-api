using Ardalis.SmartEnum;

namespace DepuChef.Application.Constants;

public sealed class ChefChoice : SmartEnum<ChefChoice>
{
    public static readonly ChefChoice Male = new(nameof(Male), 1);
    public static readonly ChefChoice Female = new(nameof(Female), 2);

    private ChefChoice(string name, int value) : base(name, value)
    {
    }
}
