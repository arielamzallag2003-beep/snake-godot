using Elysium.Foundation.Serpentis.Core;
using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using UnityEngine;

public class Main : MonoBehaviour
{
    #region Core (Logique pure)
    private SnakeGame _engine;
    private GameConfig _config;
    #endregion

    #region Références
    [Header("Scripts Spécialisés")]
    [SerializeField] private SnakeInputHandler _inputHandler;
    [SerializeField] private SnakeView _snakeView;

    [Header("Objets de la Scène 3D")]
    [SerializeField] private GameObject _appleView;
    #endregion

    void Awake() 
    {
        _config = CreateGameConfig();
        GridUtils.SetGridHeight(_config.Height);
        _engine = new SnakeGame();
        _engine.Initialize(_config, seed: 12345);

        // Passer le moteur aux autres scripts
        _snakeView.Init(_config);
        _inputHandler.Init(_engine, _snakeView);
    }

    void Update()
    {
        _engine.Update(Time.deltaTime);
        var snapshot = _engine.GetSnapshot();
        _snakeView.UpdateGraphics(snapshot);
        UpdateApple(snapshot);
    }

    private void UpdateApple(Snapshot snapshot)
    {
        _appleView.transform.position = GridUtils.CellToWorld(snapshot.Food);
    }

    private static GameConfig CreateGameConfig()
    {
        return new GameConfig(
            width: 30, height: 30, tickSeconds: 0.1,
            wrapEdges: false, initialLength: 2, fragmentChance: 0,
            entropyThresholdTicks: 9999999, engravingLifespanRuns: 0, safeSpawnPadding: 0
        );
    }
}