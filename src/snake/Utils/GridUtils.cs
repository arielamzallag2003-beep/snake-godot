using Godot;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Config;

public static class GridUtils
{
    public  static Vector2 CellToWorld(Cell cell, Sprite2D background, GameConfig config)
    {
        var size = background.GetRect().Size;
        float cellSizeX = size.X / config.Width;
        float cellSizeY = size.Y / config.Height;
        //centre l'image
        float offsetX = cellSizeX / 2f;
        float offsetY = cellSizeY / 2f;
        //converti en pixel
        return new Vector2(cell.X * cellSizeX + offsetX, cell.Y * cellSizeY + offsetY);
    }

    public static Vector2 CellToWorldTest(Cell cell, Sprite2D background, GameConfig config)
    {
        // Taille réelle à l’écran (prend le scale en compte)
        var size = background.GetRect().Size * background.Scale;

        float cellSizeX = size.X / config.Width;
        float cellSizeY = size.Y / config.Height;

        // Décalage au centre de chaque case
        float offsetX = cellSizeX / 2f;
        float offsetY = cellSizeY / 2f;

        //// Décalage selon la position réelle du Sprite (utile si Centered=true)
        //var origin = background.GlobalPosition - size / 2f;

        return new Vector2(
            cell.X * cellSizeX + offsetX,
            cell.Y * cellSizeY + offsetY
        );
    }
}