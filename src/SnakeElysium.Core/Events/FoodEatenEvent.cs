namespace Elysium.Foundation.Serpentis.Core.Events;
using Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct FoodEatenEvent(int newScore, Cell at, int length)
{
    public int NewScore { get; } = newScore;
    public Cell At { get; } = at;
    public int NewLength { get; } = length;
}
