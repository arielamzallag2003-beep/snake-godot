using UnityEngine;
using System.Collections.Generic;
using Elysium.Foundation.Serpentis.Core.Config;
using Elysium.Foundation.Serpentis.Core.Domain;

public class SnakeView : MonoBehaviour
{
    private GameConfig _config;
    private Direction _currentDirection = Direction.Right;

    [Header("Références Visuelles du Serpent")]
    [SerializeField] private GameObject _snakeHead;
    [SerializeField] private Transform _bodyContainer;
    [SerializeField] private GameObject _bodySegmentPrefab;

    private List<GameObject> _bodySegments = new List<GameObject>();

    public void Init(GameConfig config)
    {
        _config = config;
    }

    public void UpdateGraphics(Snapshot snapshot)
    {
        //  Mettre à jour la tête
        _snakeHead.transform.position = GridUtils.CellToWorld(snapshot.Snake[0]);

        // Calculer la direction basée sur les deux premiers segments
        if (snapshot.Snake.Length > 1)
        {
            Cell head = snapshot.Snake[0];
            Cell neck = snapshot.Snake[1];
            _currentDirection = GetDirectionFromCells(head, neck);
        }

        _snakeHead.transform.rotation = GetRotationForDirection(_currentDirection);

        //  Nettoyer les anciens segments
        foreach (var segment in _bodySegments)
        {
            Destroy(segment);
        }
        _bodySegments.Clear();

        //  Créer les nouveaux segments
        for (int i = 1; i < snapshot.Snake.Length; i++)
        {
            Vector3 pos = GridUtils.CellToWorld(snapshot.Snake[i]);
            GameObject segment = Instantiate(_bodySegmentPrefab, pos, Quaternion.identity, _bodyContainer);
            _bodySegments.Add(segment);
        }
    }

    private Direction GetDirectionFromCells(Cell head, Cell neck)
    {
        int dx = head.X - neck.X;
        int dy = head.Y - neck.Y;

        Debug.Log($"[DIRECTION] Head({head.X},{head.Y}) - Neck({neck.X},{neck.Y}) -> dx={dx}, dy={dy}");

        if (dx > 0)
        {
            Debug.Log("[DIRECTION] Detectee: RIGHT");
            return Direction.Right;
        }
        if (dx < 0)
        {
            Debug.Log("[DIRECTION] Detectee: LEFT");
            return Direction.Left;
        }
        if (dy > 0)
        {
            Debug.Log("[DIRECTION] Detectee: UP");
            return Direction.Up;
        }
        if (dy < 0)
        {
            Debug.Log("[DIRECTION] Detectee: DOWN");
            return Direction.Down;
        }

        Debug.LogWarning($"[DIRECTION] Aucun mouvement detecte, fallback a {_currentDirection}");
        return _currentDirection;
    }

    private Quaternion GetRotationForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Quaternion.Euler(0, 0, 0);     // Regarde vers +Z (Nord)
            case Direction.Down: return Quaternion.Euler(0, 180, 0);   // Regarde vers -Z (Sud)
            case Direction.Left: return Quaternion.Euler(0, 270, 0);   // Regarde vers -X (Ouest)
            case Direction.Right: return Quaternion.Euler(0, 90, 0);    // Regarde vers +X (Est)
            default: return Quaternion.identity;
        }
    }
}