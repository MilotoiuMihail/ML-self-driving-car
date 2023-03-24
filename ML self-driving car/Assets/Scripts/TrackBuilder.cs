using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    private Grid grid;
    private TrackPiece currentPiece;
    [SerializeField] private Transform track;
    [SerializeField] private Straight straight;
    [SerializeField] private Corner corner;
    private TrackPiece lastPieceType;
    private TrackPiece startPiece;
    private bool selectStart;
    private List<Checkpoint> checkpoints;
    private List<TrackPiece> trackPieces;
    [SerializeField] private bool trackUpDirection;
    public static TrackBuilder Instance { get; private set; }
    private void IntializeSingleton()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Awake()
    {
        IntializeSingleton();
        grid = new Grid(20, 10, 40, Vector3.zero);
        trackUpDirection = true;
    }

    private void Start()
    {
        CreatePiece(straight);
        trackPieces = new List<TrackPiece>();
        checkpoints = new List<Checkpoint>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectStart)
            {
                startPiece = (TrackPiece)grid.GetPiece(TrackBuilder.GetMouseWorldPosition());
                selectStart = false;
            }
            PlacePieceOnGrid(TrackBuilder.GetMouseWorldPosition());
            CreatePiece(lastPieceType);
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            RemovePieceFromGrid(TrackBuilder.GetMouseWorldPosition());
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePieceBy(90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotatePieceBy(-90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreatePiece(straight);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreatePiece(corner);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectStart = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ComputeTrack();
            return;
        }
    }
    private void ComputeTrack()
    {
        if (!startPiece)
        {
            return;
        }
        trackPieces?.Clear();
        checkpoints?.Clear();
        trackPieces.Add(startPiece);
        TrackPiece piece = GetSecondPiece();
        trackPieces.Add(piece);
        piece = GetNextPiece(piece);
        while (piece != startPiece && piece != null)
        {
            trackPieces.Add(piece);
            piece = GetNextPiece(piece);
        }
    }
    private TrackPiece GetSecondPiece()
    {
        TrackPiece[] neighbors = grid.GetNeighbors(startPiece.transform.position).Select(neighbor => (TrackPiece)neighbor).ToArray<TrackPiece>();
        if (startPiece.IsStraight())
        {
            if (startPiece.HasRotation(0))
            {
                return trackUpDirection ? neighbors[0] : neighbors[2];
            }
            if (startPiece.HasRotation(90))
            {
                return trackUpDirection ? neighbors[1] : neighbors[3];
            }
        }
        else if (startPiece.IsCorner())
        {
            if (startPiece.HasRotation(0))
            {
                return trackUpDirection ? neighbors[1] : neighbors[2];
            }
            if (startPiece.HasRotation(90))
            {
                return trackUpDirection ? neighbors[3] : neighbors[2];
            }
            if (startPiece.HasRotation(180))
            {
                return trackUpDirection ? neighbors[0] : neighbors[3];
            }
            if (startPiece.HasRotation(270))
            {
                return trackUpDirection ? neighbors[0] : neighbors[1];
            }
        }
        return null;
    }
    private TrackPiece GetNextPiece(TrackPiece piece)
    {
        TrackPiece[] neighbors = grid.GetNeighbors(piece.transform.position).Select(neighbor => (TrackPiece)neighbor).ToArray<TrackPiece>();
        TrackPiece previousPiece = trackPieces[trackPieces.Count - 2];
        if (piece.IsStraight())
        {
            if (piece.HasRotation(0))
            {
                return neighbors[0] != previousPiece ? neighbors[0] : neighbors[2];
            }
            if (piece.HasRotation(90))
            {
                return neighbors[1] != previousPiece ? neighbors[1] : neighbors[3];
            }
        }
        else if (piece.IsCorner())
        {
            if (piece.HasRotation(0))
            {
                return neighbors[1] != previousPiece ? neighbors[1] : neighbors[2];
            }
            if (piece.HasRotation(90))
            {
                return neighbors[2] != previousPiece ? neighbors[2] : neighbors[3];
            }
            if (piece.HasRotation(180))
            {
                return neighbors[0] != previousPiece ? neighbors[0] : neighbors[3];
            }
            if (piece.HasRotation(270))
            {
                return neighbors[0] != previousPiece ? neighbors[0] : neighbors[1];
            }
        }
        return null;
    }
    private Checkpoint[] GetCheckpoints(TrackPiece piece)
    {
        return piece?.GetComponentsInChildren<Checkpoint>();
    }
    private void CreatePiece(TrackPiece piece)
    {
        if (currentPiece)
        {
            if (currentPiece.GetType() == piece.GetType())
            {
                return;
            }
            Destroy(currentPiece.gameObject);
        }
        lastPieceType = piece;
        currentPiece = Instantiate(piece, transform.position, Quaternion.identity);
        currentPiece.transform.parent = track;
    }
    private void PlacePieceOnGrid(Vector3 position)
    {
        if (!currentPiece || !grid.TryPlacePiece(position, currentPiece))
        {
            return;
        }
        currentPiece.Place(grid.SnapPositionToGrid(position));
        currentPiece = null;
    }
    private void RemovePieceFromGrid(Vector3 position)
    {
        Destroy(grid.RemovePiece(position)?.gameObject);
    }
    private void RotatePieceBy(float degrees)
    {
        currentPiece.RotateBy(degrees);
    }
    private static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
        }
        return Vector3.zero;
    }
    public static Vector3 GetMouseSnappedPosition()
    {
        return TrackBuilder.Instance.grid.SnapPositionToGrid(TrackBuilder.GetMouseWorldPosition());
    }
    private void OnDrawGizmos()
    {
        grid?.DrawGizmos();
    }
}
