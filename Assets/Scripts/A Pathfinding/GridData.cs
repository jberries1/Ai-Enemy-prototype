using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData : MonoBehaviour
{
    [SerializeField] private Vector3 _posToStart;
    [SerializeField] private int width, height;
    [SerializeField] private float cellSize;
    [SerializeField] private List<Vector2Int> _notWalkable;
    public bool edit;
    public static GridData Instance;
    private PathFinding _pathFinding;
    public Grid<PathNode> Grid => _pathFinding.Grid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            if (_notWalkable != null && _notWalkable.Count > 0)
                _pathFinding = new PathFinding(width, height, cellSize, _posToStart, _notWalkable.ToArray());
            else
                _pathFinding = new PathFinding(width, height, cellSize, _posToStart);
        }

    }

    [ContextMenu("Generate grid")]
    private void GenerateGrid()
    {
        _pathFinding = new PathFinding(width, height, cellSize, _posToStart);
        _notWalkable = new List<Vector2Int>();
    }
    
    public void OnDrawGizmos()
    {
        if (edit)
        {
            _pathFinding ??= new PathFinding(width, height, cellSize, _posToStart, _notWalkable.ToArray());
            _pathFinding.Grid.ShowGrid();
            _pathFinding.OnDrawGizmos();
        }
    }

    private void OnValidate()
    {
        OnDrawGizmos();
    }

    public PathNode GetNode(Vector2 mousePos)
    {
        return _pathFinding.Grid.GetGridobject(mousePos);
    }

    public void AddNotWalkable(Vector2Int node)
    {
        if (!_notWalkable.Contains(node))
            _notWalkable.Add(node);
    }

    public void RemoveNotWalkable(Vector2Int node)
    {
        _notWalkable.Remove(node);
    }

    public List<Vector3> CalculatePath(Vector3 startPos, Vector3 pos)
    {
        return _pathFinding.FindPath(startPos, pos);
    }
}
