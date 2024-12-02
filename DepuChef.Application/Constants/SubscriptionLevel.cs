using Ardalis.SmartEnum;

namespace DepuChef.Application.Constants;

public sealed class SubscriptionLevel : SmartEnum<SubscriptionLevel>
{
    public static readonly SubscriptionLevel Free = new(nameof(Free), 1);
    public static readonly SubscriptionLevel Premium = new(nameof(Premium), 2);

    private SubscriptionLevel(string name, int value) : base(name, value) { }
}
