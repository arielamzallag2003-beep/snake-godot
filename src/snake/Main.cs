using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using Elysium.Foundation.Serpentis.Core.Events;
using Godot;
using Snake.SaveData;
using Snake.Views;
using System.Data;


namespace Snake
{
    public partial class Main : Node
    {
        #region Core
        private SnakeGame _engine;
        private GameConfig _config;
        private int _seed = 0;
        #endregion
        #region View
        private SnakeView _snakeView;
        private Sprite2D _appleView;
        private Node2D _entropyWallsView;
        private Sprite2D _wallSprite;
        private Sprite2D _backgroundSprite;
        private  Data dataset;
        private CanvasLayer _gameOverCanvas;
        private Button _menuButton;
        private Button _restartButton;
        private Label _score;
        #endregion
        public override void _Ready()
        {
            InitializeNodes();
            InitializeGame();
            SetupDataRecording();

            _restartButton.Pressed += OnRetryButtonPressed;
            _menuButton.Pressed += OnMenuButtonPressed;

        }
        public override void _Process(double delta)
        {
            _engine.Update(delta);
            var snapshot = _engine.GetSnapshot();
            UpdateViews(snapshot);
            UpdateWalls(snapshot);
            UpdateScore(snapshot);
            OnGameOver(snapshot);

        }

        private void InitializeNodes()
        {
            _snakeView = GetNode<SnakeView>("snake");
            _appleView = GetNode<Sprite2D>("apple");
            _backgroundSprite = GetNode<Sprite2D>("background");
            _entropyWallsView = GetNode<Node2D>("walls");

            _wallSprite = GetNode<Sprite2D>("walls/wall");
            _wallSprite.Visible = false;

            _gameOverCanvas = GetNode<CanvasLayer>("GameOver");
            _gameOverCanvas.Visible = false;

            _restartButton = GetNode<Button>("GameOver/Retry");
            _score = GetNode<Label>("Score");
        }

        private static GameConfig CreateGameConfig()
        {
            return new GameConfig(
                width: 30,
                height: 20,
                tickSeconds: 0.1,
                wrapEdges: false,
                initialLength: 2,
                fragmentChance: 0,
                entropyThresholdTicks: 100,
                engravingLifespanRuns: 0,
                safeSpawnPadding: 0
            );
        }

        private void InitializeGame()
        {
            _config = CreateGameConfig();
            _engine = new SnakeGame();
         
            _engine.Initialize(_config, seed: 12345);
            _snakeView.Init(_config, _backgroundSprite, _engine);
        }

        private void ClearWalls()
        {
            foreach (Node child in _entropyWallsView.GetChildren())
            {
                if (child != _wallSprite)
                {
                    child.QueueFree();
                }

            }
        }


        private void Restart()
        {
            foreach (Node child in _entropyWallsView.GetChildren())
            {
                if (child != _wallSprite)
                {
                    child.Free();
                }
            }

            _engine.Initialize(_config, seed: 12346);

        }

        private void UpdateViews(Snapshot snapshot)
        {
            _snakeView.UpdateGraphics(snapshot);
            _appleView.Position = GridUtils.CellToWorldTest(snapshot.Food, _backgroundSprite, _config);
        }

        private void CreateWall(Cell pos)
        {

            var wallSprite = _wallSprite.Duplicate() as Sprite2D;
            _wallSprite.Visible = true;
            wallSprite.Position = GridUtils.CellToWorldTest(pos, _backgroundSprite, _config);
            _entropyWallsView.AddChild(wallSprite);
        }

        private void UpdateWalls(Snapshot snapshot)
        {
            ClearWalls();
            foreach (var wall in snapshot.EntropyWalls)
                CreateWall(wall);
        }


        private void SetupDataRecording()
        {
            dataset = new Data();

            _engine.OnTick += (evt) =>
            {
                var snapshot = _engine.GetSnapshot();
                if (snapshot.Status == GameStatus.Running)
                    dataset.SaveData(snapshot, evt.Direction);
            };
        }
        private void ShowGameOver()
        {
            _gameOverCanvas.Visible = true;
        }
        private void OnRetryButtonPressed()
        {
            Restart();
            _gameOverCanvas.Visible = false; 
        }

        private void OnGameOver(Snapshot snapshot)
        {
            if (snapshot.Status == GameStatus.GameOver)
            {
                ShowGameOver();
            }
        }
        private void UpdateScore(Snapshot snapshot)
        {
            _score.Text = $"Score: {snapshot.Score.ToString()}";
        }

        private void OnMenuButtonPressed()
        {
            GetTree().ChangeSceneToFile("res://menu.tscn");
        }
    }

  


}

