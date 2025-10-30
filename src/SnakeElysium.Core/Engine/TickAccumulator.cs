namespace Elysium.Foundation.Serpentis.Core.Engine;

internal sealed class TickAccumulator
{
    private double _accum;
    public double TickSeconds { get; }
    public TickAccumulator(double tickSeconds)
    { if (tickSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(tickSeconds)); TickSeconds = tickSeconds; _accum = 0; }

    public int Consume(double deltaSeconds)
    {
        _accum += deltaSeconds;
        int ticks = 0;
        while (_accum >= TickSeconds) { _accum -= TickSeconds; ticks++; }
        if (_accum > TickSeconds * 4) _accum = TickSeconds;
        return ticks;
    }
    public void Reset() => _accum = 0;
}
