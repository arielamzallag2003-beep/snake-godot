namespace Elysium.Foundation.Serpentis.Core.Config;

public readonly struct GameConfig
{
    public readonly int Width;
    public readonly int Height;
    public readonly double TickSeconds;
    public readonly bool WrapEdges;
    public readonly int InitialLength;

    public readonly double FragmentChance;
    public readonly int EntropyThresholdTicks;
    public readonly int EngravingLifespanRuns;
    public readonly int SafeSpawnPadding;

    public GameConfig(
        int width, int height, double tickSeconds,
        bool wrapEdges = false, int initialLength = 3,
        double fragmentChance = 0.08, int entropyThresholdTicks = 80,
        int engravingLifespanRuns = 3,
        int safeSpawnPadding = 2)
    {
        if (width <= 3 || height <= 3) throw new ArgumentOutOfRangeException(nameof(width), "Grid too small.");
        if (tickSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(tickSeconds));
        if (initialLength < 2) throw new ArgumentOutOfRangeException(nameof(initialLength));
        if (fragmentChance is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(fragmentChance));
        if (entropyThresholdTicks < 1) throw new ArgumentOutOfRangeException(nameof(entropyThresholdTicks));
        if (engravingLifespanRuns < 0) throw new ArgumentOutOfRangeException(nameof(engravingLifespanRuns));
        if (safeSpawnPadding < 0) throw new ArgumentOutOfRangeException(nameof(safeSpawnPadding));

        Width = width;
        Height = height;
        TickSeconds = tickSeconds;
        WrapEdges = wrapEdges;
        InitialLength = initialLength;
        FragmentChance = fragmentChance;
        EntropyThresholdTicks = entropyThresholdTicks;
        EngravingLifespanRuns = engravingLifespanRuns;
        SafeSpawnPadding = safeSpawnPadding;
    }
}
