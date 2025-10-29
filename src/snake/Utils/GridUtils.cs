using Godot;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Config;

public static class GridUtils
{
  
    public static Vector2 CellToWorld(Cell cell, Sprite2D background, GameConfig config)
    {
        // Taille réelle à l’écran (prend le scale en compte)
        var size = background.GetRect().Size * background.Scale;

        float cellSizeX = size.X / config.Width;
        float cellSizeY = size.Y / config.Height;

        // Décalage au centre de chaque case
        float offsetX = cellSizeX / 2f;
        float offsetY = cellSizeY / 2f;

        //// Décalage selon la position réelle du Sprite

        return new Vector2(
            cell.X * cellSizeX + offsetX,
            cell.Y * cellSizeY + offsetY
        );
    }
}