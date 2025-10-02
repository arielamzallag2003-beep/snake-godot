internal sealed class ConsoleTime
{
    private readonly System.Diagnostics.Stopwatch _sw = System.Diagnostics.Stopwatch.StartNew();
    private double _last;

    public double NextDeltaSeconds()
    {
        var now = _sw.Elapsed.TotalSeconds;
        var dt = now - _last;
        _last = now;
        return dt;
    }
}
