namespace Elysium.Foundation.Serpentis.Core.Abstractions;

using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Events;

public interface ISnakeGame
{
    void Initialize(GameConfig config, int? seed = null);
    void Reset();
    void HandleInput(InputAction action);
    bool Update(double deltaSeconds);
    Snapshot GetSnapshot();

    event Action<GameTickEvent>? OnTick;
    event Action<FoodEatenEvent>? OnFoodEaten;
    event Action<GameOverEvent>? OnGameOver;
}
