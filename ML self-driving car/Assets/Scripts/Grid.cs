using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellsize;
    private Vector3 origin;
    private Component[,] pieces;

    public Grid(int width, int height, float cellsize, Vector3 origin)
    {
        this.pieces = new Component[width, height];
        this.width = width;
        this.height = height;
        this.cellsize = cellsize;
        this.origin = origin;
    }
    private Component[] GetNeighbors(int x, int y)
    {
        Component[] neighbors = new Component[4];
        neighbors[0] = isInBounds(x, y + 1) ? pieces[x, y + 1] : null;
        neighbors[1] = isInBounds(x + 1, y) ? pieces[x + 1, y] : null;
        neighbors[2] = isInBounds(x, y - 1) ? pieces[x, y - 1] : null;
        neighbors[3] = isInBounds(x - 1, y) ? pieces[x - 1, y] : null;
        return neighbors;
    }
    public Component[] GetNeighbors(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return GetNeighbors(gridPosition.x, gridPosition.y);
    }
    public Component GetNeighbor(Vector3 worldPosition, Vector2Int direction)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition) + direction;
        return GetPiece(gridPosition.x, gridPosition.y);
    }
    private bool TryPlacePiece(int x, int y, Component piece)
    {
        if (!isInBounds(x, y))
            return false;
        if (pieces[x, y])
            return false;
        pieces[x, y] = piece;
        return true;
    }
    public bool TryPlacePiece(Vector3 worldPosition, Component piece)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return TryPlacePiece(gridPosition.x, gridPosition.y, piece);
    }
    private Component RemovePiece(int x, int y)
    {
        if (!isInBounds(x, y))
            return null;
        if (!pieces[x, y])
            return null;
        Component piece = pieces[x, y];
        pieces[x, y] = null;
        return piece;
    }
    public Component RemovePiece(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return RemovePiece(gridPosition.x, gridPosition.y);
    }
    private bool isInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    private Component GetPiece(int x, int y)
    {
        if (!isInBounds(x, y))
            return null;
        return pieces[x, y];
    }
    public Component GetPiece(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return GetPiece(gridPosition.x, gridPosition.y);
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
        if (pieces == null)
            return;
        Gizmos.color = Color.white;
        for (int x = 0; x < pieces.GetLength(0); x++)
        {
            for (int y = 0; y < pieces.GetLength(1); y++)
            {
                Gizmos.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x, y + 1));
                Gizmos.DrawLine(GridToWorldPosition(x, y), GridToWorldPosition(x + 1, y));
            }
        }
        Gizmos.DrawLine(GridToWorldPosition(0, height), GridToWorldPosition(width, height));
        Gizmos.DrawLine(GridToWorldPosition(width, 0), GridToWorldPosition(width, height));
    }
}