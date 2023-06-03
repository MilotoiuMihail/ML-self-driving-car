using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    public int Width => width;
    public int Height => height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin;
    private Renderer gridRenderer;
    private TrackPiece[,] pieces;

    private async void OnEnable()
    {
        await System.Threading.Tasks.Task.Yield();
        GameManager.Instance.EnterEditState += Show;
        GameManager.Instance.ExitEditState += Hide;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= Show;
        GameManager.Instance.ExitEditState -= Hide;
    }
    private void Awake()
    {
        pieces = new TrackPiece[Width, Height];
        transform.position = origin;
        transform.localScale = new Vector3(Width * cellSize * .1f, 1, Height * cellSize * .1f);
        gridRenderer = GetComponentInChildren<Renderer>();
    }
    private void Start()
    {
        Hide();
    }
    private void Show()
    {
        gridRenderer.enabled = true;
    }
    private void Hide()
    {
        gridRenderer.enabled = false;
    }
    private bool isInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    private bool CanPlacePiece(int x, int y)
    {
        return isInBounds(x, y) && !pieces[x, y];
    }
    public bool CanPlacePiece(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return CanPlacePiece(gridPosition.x, gridPosition.y);
    }
    private void PlacePiece(int x, int y, TrackPiece piece)
    {
        pieces[x, y] = piece;
    }
    public void PlacePiece(Vector3 worldPosition, TrackPiece piece)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        PlacePiece(gridPosition.x, gridPosition.y, piece);
    }
    private TrackPiece RemovePiece(int x, int y)
    {
        if (!isInBounds(x, y))
            return null;
        if (!pieces[x, y])
            return null;
        TrackPiece piece = pieces[x, y];
        pieces[x, y] = null;
        return piece;
    }
    public TrackPiece RemovePiece(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return RemovePiece(gridPosition.x, gridPosition.y);
    }
    public TrackPiece GetPiece(int x, int y)
    {
        return isInBounds(x, y) ? pieces[x, y] : null;
    }
    public TrackPiece GetPiece(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return GetPiece(gridPosition.x, gridPosition.y);
    }
    public TrackPiece GetNeighbor(TrackPiece piece, Vector3 direction)
    {
        Vector2Int gridPosition = WorldToGridPosition(piece.transform.position) + new Vector2Int((int)direction.x, (int)direction.z);
        return GetPiece(gridPosition.x, gridPosition.y);
    }
    private Vector3 GridToWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + origin;
    }
    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int();
        gridPosition.x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        gridPosition.y = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
        return gridPosition;
    }
    public Vector3 SnapPositionToGrid(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        Vector3 snappedposition = GridToWorldPosition(gridPosition.x, gridPosition.y);
        return snappedposition + new Vector3(.5f, 0, .5f) * cellSize;
    }
    public void Reset()
    {
        foreach (TrackPiece piece in pieces)
        {
            if (piece)
            {
                Destroy(piece.gameObject);
            }
        }
        pieces = new TrackPiece[Width, Height];
    }
}
