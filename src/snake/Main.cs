using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using Godot;

public partial class Main : Node
{
    private SnakeGame _engine;
    private GameConfig _config;
    
    private SnakeView _snakeView;
    private Sprite2D _appleView;
    private Sprite2D _backgroundSprite;
    public override void _Ready()
    {

        _snakeView = GetNode<SnakeView>("snake");
        _appleView = GetNode<Sprite2D>("apple");
        _backgroundSprite = GetNode<Sprite2D>("background");

        _config = new GameConfig(
            width: 30,
            height: 20,
            tickSeconds: 0.2,
            wrapEdges: false,
            initialLength: 2,
            fragmentChance: 0,
            entropyThresholdTicks: 999999999,
            engravingLifespanRuns: 0,
            safeSpawnPadding: 0
        );

       
        _engine = new SnakeGame();

        _engine.OnGameOver += (evt) => {
            GD.PrintErr($"🔴 GAME OVER : {evt.Reason}");
            GD.PrintErr($"   Tick: {evt.TickCount}, Score: {evt.Score}");
        };

        _engine.Initialize(_config, seed: 12345);
        _snakeView.Init(_config, _backgroundSprite, _engine);

        

    }
    public override void _Process(double delta)
    {
        _engine.Update(delta);
        var snapshot = _engine.GetSnapshot();
        _snakeView.UpdateGraphics(snapshot);
        var apple = snapshot.Food;
        _appleView.Position = GridUtils.CellToWorldTest(apple,_backgroundSprite,_config);

    }
}
