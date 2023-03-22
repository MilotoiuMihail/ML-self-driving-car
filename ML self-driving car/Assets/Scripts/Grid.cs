using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellsize;
    private Vector3 origin;
    private GameObject[,] items;

    public Grid(int width, int height, float cellsize, Vector3 origin)
    {
        this.items = new GameObject[width, height];
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        this.origin = origin;
    }
    private GameObject[] GetNeighbors(int x, int y)
    {
        GameObject[] neighbors = new GameObject[4];
        neighbors[0] = isInBounds(x, y + 1) ? items[x, y + 1] : null;
        neighbors[1] = isInBounds(x + 1, y) ? items[x + 1, y] : null;
        neighbors[2] = isInBounds(x, y - 1) ? items[x, y - 1] : null;
        neighbors[3] = isInBounds(x - 1, y) ? items[x - 1, y] : null;
        return neighbors;
    }
    public GameObject[] GetNeighbors(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return GetNeighbors(gridPosition.x, gridPosition.y);
    }
    private bool TryPlaceItem(int x, int y, GameObject item)
    {
        if (!isInBounds(x, y))
            return false;
        if (items[x, y])
            return false;
        items[x, y] = item;
        return true;
    }
    public bool TryPlaceItem(Vector3 worldPosition, GameObject item)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return TryPlaceItem(gridPosition.x, gridPosition.y, item);
    }
    private GameObject RemoveItem(int x, int y)
    {
        if (!isInBounds(x, y))
            return null;
        if (!items[x, y])
            return null;
        GameObject item = items[x, y];
        items[x, y] = null;
        return item;
    }
    public GameObject RemoveItem(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return RemoveItem(gridPosition.x, gridPosition.y);
    }
    private bool isInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    private Vector3 GridToWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellsize + origin;
    }
    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int();
        gridPosition.x = Mathf.FloorToInt((worldPosition - origin).x / cellsize);
        gridPosition.y = Mathf.FloorToInt((worldPosition - origin).z / cellsize);
        return gridPosition;
    }
    public Vector3 SnapPositionToGrid(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        Vector3 snappedposition = GridToWorldPosition(gridPosition.x, gridPosition.y);
        return snappedposition + new Vector3(.5f, 0, .5f) * cellsize;
    }

    public void DrawGizmos()
    {
        if (items == null)
            return;
        Gizmos.color = Color.white;
        for (int x = 0; x < items.GetLength(0); x++)
        {
            for (int y = 0; y < items.GetLength(1); y++)
            {
                Gizmos.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x, y + 1));
                Gizmos.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x + 1, y));
            }
        }
        Gizmos.DrawLine(GridToWorldPosition(0, height), GridToWorldPosition(width, height));
        Gizmos.DrawLine(GridToWorldPosition(width, 0), GridToWorldPosition(width, height));
    }
}
