namespace Elysium.Foundation.Serpentis.Core.Domain;

public readonly struct Cell : IEquatable<Cell>
{
    public readonly int X;
    public readonly int Y;
    public Cell(int x, int y) { X = x; Y = y; }

    public bool Equals(Cell other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Cell c && Equals(c);
    public override int GetHashCode() => unchecked((X * 397) ^ Y);
    public override string ToString() => $"({X},{Y})";
    public static bool operator ==(Cell a, Cell b) => a.Equals(b);
    public static bool operator !=(Cell a, Cell b) => !a.Equals(b);
}
