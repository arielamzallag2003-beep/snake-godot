namespace Elysium.Foundation.Serpentis.Core.Events;
using Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct GameTickEvent(int tickCount, Direction direction)
{
    public int TickCount { get; } = tickCount;
    public Direction Direction { get; } = direction;
}
