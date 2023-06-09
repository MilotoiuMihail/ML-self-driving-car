using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Track : MonoBehaviour
{
    private List<TrackPiece> track = new List<TrackPiece>();
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public TrackPiece StartPiece { get; set; }
    public event Action<bool> IsTrackDirectionClockwiseChanged;
    private bool isTrackDirectionClockwise = true;
    public bool IsTrackDirectionClockwise
    {
        get { return isTrackDirectionClockwise; }
        set
        {
            if (isTrackDirectionClockwise != value)
            {
                isTrackDirectionClockwise = value;
                IsTrackDirectionClockwiseChanged?.Invoke(value);
            }
        }
    }
    public event Action<bool> SelectStartChanged;
    private bool selectStart;
    public bool SelectStart
    {
        get { return selectStart; }
        set
        {
            if (selectStart != value)
            {
                selectStart = value;
                SelectStartChanged?.Invoke(value);
            }
        }
    }
    private Grid grid;
    [SerializeField] private TrackEditor trackEditor;
    public TrackEditor TrackEditor => trackEditor;
    private void OnEnable()
    {
        TrackEditor.PiecePlaced += SelectStartPiece;
    }
    private void OnDisable()
    {
        TrackEditor.PiecePlaced -= SelectStartPiece;
    }
    private void Awake()
    {
        grid = TrackEditor.Grid;
    }
    private void Start()
    {
        TrackDataManager.LoadTrack(this);
    }
    public List<Checkpoint> GetCheckpoints()
    {
        return checkpoints;
    }
    public void SelectStartPiece()
    {
        if (!StartPiece || SelectStart)
        {
            StartPiece = grid.GetPiece(InputManager.Instance.MousePosition);
            SelectStart = false;
        }
    }
    public void SetTrackDirection(bool trackDirection)
    {
        IsTrackDirectionClockwise = trackDirection;
    }
    public void ComputeTrack()
    {
        if (!StartPiece)
        {
            return;
        }
        ResetTrack();
        track.Add(StartPiece);
        TrackPiece piece = GetNextTrackPiece(StartPiece, IsTrackDirectionClockwise);
        while (piece != StartPiece && piece != null)
        {
            track.Add(piece);
            piece = GetCurrentNextTrackPiece(piece);
        }
    }
    private void ResetTrack()
    {
        track?.Clear();
        checkpoints?.Clear();
    }
    private TrackPiece GetNextTrackPiece(TrackPiece piece, bool isGoingForward)
    {
        if (isGoingForward)
        {
            checkpoints.AddRange(piece.GetCheckpoints());
            if (piece.IsStraight())
            {
                return grid.GetNeighbor(piece, piece.transform.forward);
            }
            return grid.GetNeighbor(piece, piece.transform.right);
        }
        checkpoints.AddRange(piece.GetCheckpoints().Reverse());
        return grid.GetNeighbor(piece, -piece.transform.forward);
    }
    private TrackPiece GetCurrentNextTrackPiece(TrackPiece piece)
    {
        TrackPiece previousPiece = track[track.Count - 2];
        TrackPiece backPiece = grid.GetNeighbor(piece, -piece.transform.forward);
        return GetNextTrackPiece(piece, previousPiece == backPiece);
    }
    public bool IsValidStartPiece()
    {
        return StartPiece;
    }
    public bool HasValidTrackLength()
    {
        return track.Count > 3;
    }
}
