namespace Elysium.Foundation.Serpentis.Core.Events;
using Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct GameOverEvent(GameOverReason reason, int tickCount, int score)
{
    public GameOverReason Reason { get; } = reason;
    public int TickCount { get; } = tickCount;
    public int Score { get; } = score;
}
