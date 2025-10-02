using Elysium.Foundation.Serpentis.Core.Domain;

internal sealed class ConsoleRenderer
{
    private readonly int _w, _h;

    public ConsoleRenderer(int w, int h)
    {
        _w = w; _h = h;
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Draw(Snapshot s)
    {
        Console.SetCursorPosition(0, 0);

        Console.Write('+');
        for (int x = 0; x < _w; x++) Console.Write('-');
        Console.WriteLine('+');

        var snakeArr = s.Snake;
        var engrSet  = s.Engravings.Length == 0 ? null : new HashSet<Cell>(s.Engravings);
        var entSet   = s.EntropyWalls.Length == 0 ? null : new HashSet<Cell>(s.EntropyWalls);
        var safeSet  = s.SafeTiles.Length == 0 ? null : new HashSet<Cell>(s.SafeTiles);
        var snakeSet = snakeArr.Length == 0 ? null : new HashSet<Cell>(snakeArr);

        for (int y = 0; y < _h; y++)
        {
            Console.Write('|');
            for (int x = 0; x < _w; x++)
            {
                var cell = new Cell(x, y);

                if (s.HasFood && s.Food == cell)
                {
                    if (s.FoodKind == FoodKind.Fragment) { Console.ForegroundColor = ConsoleColor.Blue; Console.Write('^'); Console.ResetColor(); }
                    else { Console.ForegroundColor = ConsoleColor.Cyan; Console.Write('*'); Console.ResetColor(); }
                    continue;
                }

                if (snakeArr.Length > 0 && snakeArr[0] == cell) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write('O'); Console.ResetColor(); continue; }
                if (snakeSet != null && snakeSet.Contains(cell)) { Console.ForegroundColor = ConsoleColor.Green; Console.Write('o'); Console.ResetColor(); continue; }
                if (entSet != null && entSet.Contains(cell)) { Console.ForegroundColor = ConsoleColor.Red; Console.Write('#'); Console.ResetColor(); continue; }
                if (engrSet != null && engrSet.Contains(cell)) { Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write('.'); Console.ResetColor(); continue; }

                if (safeSet != null && safeSet.Contains(cell)) { Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write('·'); Console.ResetColor(); continue; }

                Console.Write(' ');
            }
            Console.WriteLine('|');
        }

        Console.Write('+');
        for (int x = 0; x < _w; x++) Console.Write('-');
        Console.WriteLine('+');

        Console.WriteLine();

        WriteHudLine($"Tick:{s.TickCount}  Score:{s.Score}  Status:{s.Status}");
        if (s.Status == GameStatus.GameOver)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteHudLine("Game Over — press R to Restart, Esc to Quit.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteHudLine("Arrows/ZQSD/WASD=Move  P=Pause  R=Restart  Esc=Quit");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        WriteHudLine("Legend: O/o Snake  * Core  ^ Fragment  # Entropy  . Engraving  · Safe Zone");
        Console.ResetColor();
    }

    private void WriteHudLine(string text)
    {
        int totalWidth = _w + 4;
        Console.WriteLine(text.PadRight(totalWidth));
    }
}
