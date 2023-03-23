using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    private Grid grid;
    private TrackPiece currentPiece;
    [SerializeField] private Transform track;
    [SerializeField] private Straight straight;
    [SerializeField] private Corner corner;
    private TrackPiece lastPiece;
    private TrackPiece startPiece;
    private bool selectStart;
    private List<Checkpoint> checkpoints;
    private List<TrackPiece> trackPieces;
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
    }

    private void Start()
    {
        CreatePiece(straight);
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
            CreatePiece(lastPiece);
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
    }
    private void GetTrackPieces()
    {
        if (!startPiece)
        {
            return;
        }
        trackPieces.Add(startPiece);
        TrackPiece piece = GetNextPiece(startPiece);
        while (piece != startPiece)
        {
            trackPieces.Add(piece);
            piece = GetNextPiece(piece);
        }
    }
    private TrackPiece GetNextPiece(TrackPiece piece)
    {
        return null;
    }
    // private void GetCheckpoints()
    // {
    //     if (!startPiece)
    //     {
    //         return;
    //     }
    //     checkpoints.Add(startPiece.GetComponentsInChildren<Checkpoint>()[0]);
    //     TrackSegment piece = GetNextPiece(startPiece);
    //     while (piece != startPiece.GetComponent<TrackSegment>())
    //     {
    //         checkpoints.AddRange(piece.GetComponentsInChildren<Checkpoint>());
    //         piece = GetNextPiece(piece);
    //     }
    //     checkpoints.Add(startPiece.GetComponentsInChildren<Checkpoint>()[1]);
    // }
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
        lastPiece = piece;
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
        Destroy(grid.RemovePiece(position).gameObject);
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
