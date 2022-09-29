using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGrigObject>
{
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public Vector3 OriginPosition { get; private set; }
    public float CellSize { get; private set; }
    private int _width, _height;
    private TGrigObject[,] _gridArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGrigObject>, int, int, TGrigObject> createNewGridObject)
    {
        _height = height;
        _width = width;
        CellSize = cellSize;
        OriginPosition = originPosition;
        _gridArray = new TGrigObject[width, height];
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                _gridArray[x, y] = createNewGridObject(this, x, y);
            }
        }
    }

    public Vector3 GetWorldPosition(int x, int y) => new Vector3(x, OriginPosition.y, y) * CellSize + OriginPosition;

    public int GetWidth() => _gridArray.GetLength(0);

    public int GetHeight() => _gridArray.GetLength(1);

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
        int y = Mathf.FloorToInt((worldPosition.z - OriginPosition.z) / CellSize);
        return new Vector2Int(x, y);
    }
    
    public Vector2Int GetXYEditor(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize);
        int y = Mathf.FloorToInt((worldPosition.y - OriginPosition.z) / CellSize);
        return new Vector2Int(x, y);
    }

    public void SetGridObject(int x, int y, TGrigObject value)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _gridArray[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGrigObject value)
    {
        Vector2Int local = GetXY(worldPosition);
        SetGridObject(local.x, local.y, value);
    }

    public void TriggerGridChanged(int x, int y)
    {
        if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public TGrigObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
            return _gridArray[x, y];
        else
            return default;
    }

    public TGrigObject GetGridobject(Vector3 worldPosition)
    {
        Vector2Int local = GetXYEditor(worldPosition);
        return GetGridObject(local.x, local.y);
    }

    public void ShowGrid()
    {
        //Debug
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 0.5f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 0.5f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 0.5f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 0.5f);
    }
}
