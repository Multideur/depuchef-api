using Ardalis.SmartEnum;

namespace DepuChef.Application.Constants;

public sealed class ChefChoice : SmartEnum<ChefChoice>
{
    public static readonly ChefChoice Femi = new(nameof(Femi), 1);
    public static readonly ChefChoice Tyler = new(nameof(Tyler), 2);
    public static readonly ChefChoice Tasha = new(nameof(Tasha), 3);
    public static readonly ChefChoice Anna = new(nameof(Anna), 4);

    private ChefChoice(string name, int value) : base(name, value)
    {
    }
}
