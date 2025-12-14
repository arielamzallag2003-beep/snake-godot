using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Engine;
using Elysium.Foundation.Serpentis.Core.Events;
using Godot;
using Snake.SaveData;
using Snake.Views;

using Snake.AI;

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
        #endregion
        #region UI
        private CanvasLayer _gameOverCanvas;
        private Button _menuButton;
        private Button _restartButton;
        private Label _score;
        #endregion
        #region Audio
        private AudioStreamPlayer _eatSound;
        #endregion


        private SnakeIA _brain;
        private bool _useAI = true;
        

        public override void _Ready()
        {
            InitializeNodes();
            InitializeGame();
            SetupDataRecording();

            _useAI = GameData.UseAI;
            _brain = new SnakeIA();
            AddChild(_brain);

            _restartButton.Pressed += OnRetryButtonPressed;

            _menuButton.Pressed += OnMenuButtonPressed;
            _engine.OnFoodEaten += PlayEatSound;

            if (!string.IsNullOrEmpty(GameData.DatasetPath))
            {
                GD.Print($"Chargement du dataset choisi depuis le menu : {GameData.DatasetPath}");
                _brain.TrainModel(GameData.DatasetPath);
            }
            else
            {
                _brain.TrainModel();
            }

        }
        public override void _Process(double delta)
        {
            if (_engine.GetSnapshot().Status == GameStatus.Running)
            {
                if (_useAI)
                {
                    var snapshot_ = _engine.GetSnapshot();
                    Direction aiMove = _brain.PredictMove(snapshot_);
                    ApplyAIDirection(aiMove);
                }
            }
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
            _menuButton = GetNode<Button>("GameOver/Menu");
            _score = GetNode<Label>("Score");

            _eatSound = GetNode<AudioStreamPlayer>("AppleEatSound");
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
                entropyThresholdTicks: 99999,
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
            _appleView.Position = GridUtils.CellToWorld(snapshot.Food, _backgroundSprite, _config);
        }

        private void CreateWall(Cell pos)
        {

            var wallSprite = _wallSprite.Duplicate() as Sprite2D;
            _wallSprite.Visible = true;
            wallSprite.Position = GridUtils.CellToWorld(pos, _backgroundSprite, _config);
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
                if (_useAI) return;
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
            if (snapshot.Status == GameStatus.GameOver) ShowGameOver();
               
        }
        private void UpdateScore(Snapshot snapshot)
        {
            _score.Text = $"Score: {snapshot.Score.ToString()}";
        }

        private void OnMenuButtonPressed()
        {
            GetTree().ChangeSceneToFile("res://scene/menu.tscn");
        }

        private void PlayEatSound(FoodEatenEvent eventData)
        {
            _eatSound.Play();
        }
        private void ApplyAIDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: _engine.HandleInput(InputAction.TurnUp); break;
                case Direction.Down: _engine.HandleInput(InputAction.TurnDown); break;
                case Direction.Left: _engine.HandleInput(InputAction.TurnLeft); break;
                case Direction.Right: _engine.HandleInput(InputAction.TurnRight); break;
            }
        }
        
    }
}

