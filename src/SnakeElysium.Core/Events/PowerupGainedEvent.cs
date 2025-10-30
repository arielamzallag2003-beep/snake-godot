namespace Elysium.Foundation.Serpentis.Core.Events;

public readonly struct PowerupGainedEvent(int nextFoodMultiplier)
{
    public int NextFoodMultiplier { get; } = nextFoodMultiplier;
}
