namespace Elysium.Foundation.Serpentis.Core.Engine;
using Elysium.Foundation.Serpentis.Core.Abstractions;

public sealed class DeterministicRandom : IRandom
{
    private int _state;
    public DeterministicRandom(int seed) { _state = seed == 0 ? 1 : seed; }
    public int Next(int minInclusive, int maxExclusive)
    {
        _state = unchecked(_state * 1664525 + 1013904223);
        var v = (uint)_state >> 1;
        return minInclusive + (int)(v % (uint)(maxExclusive - minInclusive));
    }
}
