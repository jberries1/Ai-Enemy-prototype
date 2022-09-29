public class PathNode
{
    private Grid<PathNode> grid;
    private int _x, _y;
    public int x => _x;
    public int y => _y;

    public int gCost, hCost, fCost;

    public bool isWalkable;

    public PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        _x = x;
        _y = y;
        isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return _x + "," + _y;
    }
}