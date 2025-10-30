using UnityEngine;
using Elysium.Foundation.Serpentis.Core.Domain;
using Elysium.Foundation.Serpentis.Core.Config;

public static class GridUtils
{
    
    private static int _gridHeight = 20; 

   
    public static Vector3 CellToWorld(Cell cell)
    {
        // On inverse Y : plus Y est grand dans la logique, plus on va vers le BAS en 3D
        return new Vector3(cell.X, 0.5f, (_gridHeight - 1) - cell.Y);
    }

   
    public static void SetGridHeight(int height)
    {
        _gridHeight = height;
    }
}