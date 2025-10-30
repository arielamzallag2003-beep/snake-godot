namespace Elysium.Foundation.Serpentis.Core.Events;

using Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct EntropyWallSpawnedEvent(Cell whereAt, int ticksSinceLastCore)
{
    public Cell WhereAt { get; } = whereAt;
    public int TicksSinceLastCore { get; } = ticksSinceLastCore;
}
