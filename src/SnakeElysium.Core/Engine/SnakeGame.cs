namespace Elysium.Foundation.Serpentis.Core.Engine;

using System.Collections.Generic;
using System.Linq;
using Elysium.Foundation.Serpentis.Core.Abstractions;
using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Events;

public sealed class SnakeGame : ISnakeGame
{
    private GameConfig _config;
    private GridSize _grid;
    private IRandom _rng = new DeterministicRandom(1);

    private readonly List<Cell> _snake = new(capacity: 128);
    private readonly HashSet<Cell> _occupied = new();
    private readonly Dictionary<Cell, int> _engravingsLife = new();
    private readonly HashSet<Cell> _entropyWalls = new();
    private readonly HashSet<Cell> _safeTiles = new();

    private Direction _direction = Direction.Right;
    private Direction _pendingDirection = Direction.Right;

    private Cell _food;
    private bool _hasFood;
    private FoodKind _foodKind = FoodKind.Core;

    private GameStatus _status = GameStatus.Running;
    private int _score;
    private int _tickCount;

    private TickAccumulator _accum = new(0.2);
    private int? _seed;

    private int _ticksSinceLastCoreEat = 0;
    private int _nextFoodScoreMultiplier = 1;

    public event Action<GameTickEvent>? OnTick;
    public event Action<FoodEatenEvent>? OnFoodEaten;
    public event Action<GameOverEvent>? OnGameOver;
    public event Action<PowerupGainedEvent>? OnPowerupGained;
    public event Action<EntropyWallSpawnedEvent>? OnEntropyWallSpawned;

    public void Initialize(GameConfig config, int? seed = null)
    {
        _config = config;
        _grid = new GridSize(config.Width, config.Height);
        _accum = new TickAccumulator(config.TickSeconds);
        _seed = seed ?? Environment.TickCount;
        _rng = new DeterministicRandom(_seed.Value);
        BuildSafeTiles();
        SetupInitialState();
    }

    public void Reset()
    {
        _rng = new DeterministicRandom((_seed ?? 1));
        SetupInitialState();
    }

    private void SetupInitialState()
    {
        if (_engravingsLife.Count > 0)
        {
            var toRemove = new List<Cell>();
            foreach (var kv in _engravingsLife)
            {
                int remaining = kv.Value - 1;
                if (remaining <= 0) toRemove.Add(kv.Key);
                else _engravingsLife[kv.Key] = remaining;
            }
            foreach (var c in toRemove) _engravingsLife.Remove(c);
        }

        _snake.Clear();
        _occupied.Clear();
        _entropyWalls.Clear();

        _score = 0;
        _tickCount = 0;
        _ticksSinceLastCoreEat = 0;
        _nextFoodScoreMultiplier = 1;

        _status = GameStatus.Running;
        _direction = Direction.Right;
        _pendingDirection = Direction.Right;
        _hasFood = false;

        int cx = _grid.Width / 2;
        int cy = _grid.Height / 2;
        int len = System.Math.Min(_config.InitialLength, System.Math.Max(2, _grid.Width - 2));
        for (int i = 0; i < len; i++)
            _snake.Add(new Cell(cx - i, cy));
        _occupied.UnionWith(_snake);

        foreach (var c in _snake)
            _engravingsLife.Remove(c);

        SpawnFoodOrEnd();
        _accum.Reset();
    }

    public void HandleInput(InputAction action)
    {
        if (action == InputAction.Restart)
        {
            Reset();
            return;
        }
        if (_status == GameStatus.GameOver) return;

        switch (action)
        {
            case InputAction.PauseToggle:
                _status = _status == GameStatus.Running ? GameStatus.Paused : GameStatus.Running;
                break;
            case InputAction.TurnUp: SetPending(Direction.Up); break;
            case InputAction.TurnDown: SetPending(Direction.Down); break;
            case InputAction.TurnLeft: SetPending(Direction.Left); break;
            case InputAction.TurnRight: SetPending(Direction.Right); break;
        }
    }

    public bool Update(double deltaSeconds)
    {
        if (_status != GameStatus.Running) return false;

        int ticks = _accum.Consume(deltaSeconds);
        bool progressed = false;

        for (int i = 0; i < ticks && _status == GameStatus.Running; i++)
        {
            StepOnce();
            progressed = true;

            _ticksSinceLastCoreEat++;
            if (_ticksSinceLastCoreEat >= _config.EntropyThresholdTicks)
            {
                if (TrySpawnEntropyWall(out var where))
                    OnEntropyWallSpawned?.Invoke(new EntropyWallSpawnedEvent(where, _ticksSinceLastCoreEat));
                _ticksSinceLastCoreEat = 0;
            }
        }
        return progressed;
    }

    public Snapshot GetSnapshot()
    {
        var copySnake = _snake.ToArray();
        var copyEngr = _engravingsLife.Count == 0 ? Array.Empty<Cell>() : _engravingsLife.Keys.ToArray();
        var copyEntropy = _entropyWalls.Count == 0 ? Array.Empty<Cell>() : _entropyWalls.ToArray();
        var copySafe = _safeTiles.Count == 0 ? Array.Empty<Cell>() : _safeTiles.ToArray();

        return new Snapshot(
            _status,
            _score,
            copySnake,
            _food,
            _hasFood,
            _foodKind,
            _tickCount,
            _grid,
            copyEngr,
            copyEntropy,
            copySafe
        );
    }

    private void SetPending(Direction dir)
    {
        if (_snake.Count > 1 && IsOpposite(_direction, dir)) return;
        _pendingDirection = dir;
    }

    private static bool IsOpposite(Direction a, Direction b) =>
        (a == Direction.Up && b == Direction.Down) ||
        (a == Direction.Down && b == Direction.Up) ||
        (a == Direction.Left && b == Direction.Right) ||
        (a == Direction.Right && b == Direction.Left);

    private void StepOnce()
    {
        if (!IsOpposite(_direction, _pendingDirection) || _snake.Count == 1)
            _direction = _pendingDirection;

        var head = _snake[0];
        var delta = _direction switch
        {
            Direction.Up => new Cell(0, -1),
            Direction.Down => new Cell(0, 1),
            Direction.Left => new Cell(-1, 0),
            Direction.Right => new Cell(1, 0),
            _ => new Cell(1, 0)
        };

        int nx = head.X + delta.X;
        int ny = head.Y + delta.Y;

        if (_config.WrapEdges)
        {
            nx = (nx % _grid.Width + _grid.Width) % _grid.Width;
            ny = (ny % _grid.Height + _grid.Height) % _grid.Height;
        }
        else
        {
            if (nx < 0 || ny < 0 || nx >= _grid.Width || ny >= _grid.Height)
            {
                EndGame(GameOverReason.WallCollision);
                return;
            }
        }

        var newHead = new Cell(nx, ny);
        bool eats = _hasFood && newHead == _food;

        if (!eats && (_occupied.Contains(newHead) || _engravingsLife.ContainsKey(newHead) || _entropyWalls.Contains(newHead)))
        {
            EndGame(GameOverReason.SelfCollision);
            return;
        }

        _snake.Insert(0, newHead);
        _occupied.Add(newHead);

        if (eats)
        {
            if (_foodKind == FoodKind.Core)
            {
                _score += 1 * _nextFoodScoreMultiplier;
                _nextFoodScoreMultiplier = 1;
                _hasFood = false;
                _ticksSinceLastCoreEat = 0;
                OnFoodEaten?.Invoke(new FoodEatenEvent(_score, _food, _snake.Count));
                SpawnFoodOrEnd();
            }
            else
            {
                _nextFoodScoreMultiplier = System.Math.Max(_nextFoodScoreMultiplier, 2);
                _hasFood = false;
                OnPowerupGained?.Invoke(new PowerupGainedEvent(_nextFoodScoreMultiplier));
                SpawnFoodOrEnd();
                var tailFrag = _snake[^1];
                _snake.RemoveAt(_snake.Count - 1);
                _occupied.Remove(tailFrag);
            }
        }
        else
        {
            var tail = _snake[^1];
            _snake.RemoveAt(_snake.Count - 1);
            _occupied.Remove(tail);
        }

        _tickCount++;
        OnTick?.Invoke(new GameTickEvent(_tickCount, _direction));
    }

    private void SpawnFoodOrEnd()
    {
        int free = _grid.Width * _grid.Height - _snake.Count - _engravingsLife.Count - _entropyWalls.Count - _safeTiles.Count;
        if (free <= 0) { EndGame(GameOverReason.NoSpaceLeft); return; }

        _foodKind = (_rng.Next(0, 10_000) < (int)(_config.FragmentChance * 10_000))
            ? FoodKind.Fragment
            : FoodKind.Core;

        int targetIndex = _rng.Next(0, free);
        int idx = 0;

        for (int y = 0; y < _grid.Height; y++)
        for (int x = 0; x < _grid.Width; x++)
        {
            var c = new Cell(x, y);
            if (_occupied.Contains(c) || _engravingsLife.ContainsKey(c) || _entropyWalls.Contains(c) || _safeTiles.Contains(c))
                continue;

            if (idx == targetIndex)
            {
                _food = c;
                _hasFood = true;
                return;
            }
            idx++;
        }

        _food = new Cell(0, 0);
        _hasFood = true;
    }

    private bool TrySpawnEntropyWall(out Cell where)
    {
        var invalidSpawns = new HashSet<Cell>(_occupied);       // Le corps du serpent
        invalidSpawns.UnionWith(_engravingsLife.Keys);    // Les fantômes
        invalidSpawns.UnionWith(_entropyWalls);           // Les murs existants
        invalidSpawns.UnionWith(_safeTiles);              // La zone de départ
        if (_hasFood) invalidSpawns.Add(_food);           // La pomme
        if (_snake.Count > 0)
        {
            var head = _snake[0];
            invalidSpawns.Add(new Cell(head.X, head.Y - 1)); // Up
            invalidSpawns.Add(new Cell(head.X, head.Y + 1)); // Down
            invalidSpawns.Add(new Cell(head.X - 1, head.Y)); // Left
            invalidSpawns.Add(new Cell(head.X + 1, head.Y)); // Right
        }

        int reserve = _snake.Count + _engravingsLife.Count + _entropyWalls.Count + _safeTiles.Count + (_hasFood ? 1 : 0);
        int free = _grid.Width * _grid.Height - reserve;
        if (free <= 0) { where = default; return false; }

        int targetIndex = _rng.Next(0, free);
        int idx = 0;

        for (int y = 0; y < _grid.Height; y++)
        for (int x = 0; x < _grid.Width; x++)
        {
            var c = new Cell(x, y);
            if (_occupied.Contains(c) || _engravingsLife.ContainsKey(c) || _entropyWalls.Contains(c) || _safeTiles.Contains(c) || (_hasFood && c == _food))
                continue;

            if (idx == targetIndex)
            {
                _entropyWalls.Add(c);
                where = c;
                return true;
            }
            idx++;
        }

        where = default;
        return false;
    }

    private void EndGame(GameOverReason reason)
    {
        int life = System.Math.Max(0, _config.EngravingLifespanRuns);
        if (life > 0)
        {
            foreach (var c in _snake)
            {
                if (!_safeTiles.Contains(c))
                    _engravingsLife[c] = life;
            }
        }

        _status = GameStatus.GameOver;
        OnGameOver?.Invoke(new GameOverEvent(reason, _tickCount, _score));
    }

    private void BuildSafeTiles()
    {
        _safeTiles.Clear();
        int pad = _config.SafeSpawnPadding;
        if (pad <= 0) return;

        int cx = _grid.Width / 2;
        int cy = _grid.Height / 2;
        int len = System.Math.Min(_config.InitialLength, System.Math.Max(2, _grid.Width - 2));

        int minX = Clamp(cx - (len - 1) - pad, 0, _grid.Width - 1);
        int maxX = Clamp(cx + pad, 0, _grid.Width - 1);
        int minY = Clamp(cy - pad, 0, _grid.Height - 1);
        int maxY = Clamp(cy + pad, 0, _grid.Height - 1);

        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                _safeTiles.Add(new Cell(x, y));
    }

    private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
}
