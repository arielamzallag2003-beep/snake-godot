namespace Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct Snapshot
{
    public GameStatus Status { get; }
    public int Score { get; }
    public Cell[] Snake { get; }
    public Cell Food { get; }
    public bool HasFood { get; }
    public FoodKind FoodKind { get; }
    public int TickCount { get; }
    public GridSize Grid { get; }
    public Cell[] Engravings { get; }
    public Cell[] EntropyWalls { get; }
    public Cell[] SafeTiles { get; }

    public Snapshot(
        GameStatus status, int score, Cell[] snake,
        Cell food, bool hasFood, FoodKind foodKind,
        int tickCount, GridSize grid,
        Cell[] engravings, Cell[] entropyWalls,
        Cell[] safeTiles)
    {
        Status = status;
        Score = score;
        Snake = snake;
        Food = food;
        HasFood = hasFood;
        FoodKind = foodKind;
        TickCount = tickCount;
        Grid = grid;
        Engravings = engravings;
        EntropyWalls = entropyWalls;
        SafeTiles = safeTiles;
    }
}
