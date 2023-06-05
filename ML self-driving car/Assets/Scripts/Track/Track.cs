using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Track : MonoBehaviour
{
    public event Action StartPieceChanged;
    private TrackPiece startPiece;
    public TrackPiece StartPiece
    {
        get { return startPiece; }
        set
        {
            if (startPiece == value)
            {
                return;
            }
            if (StartPiece && StartPiece.IsFacingForward != isTrackDirectionClockwise)
            {
                isTrackDirectionClockwise = StartPiece.IsFacingForward;
            }
            startPiece = value;
            if (!track.Contains(StartPiece))
            {
                ComputeTrack();
            }
            // StartPiece.IsFacingForward = IsTrackDirectionClockwise;
            StartPieceChanged?.Invoke();
        }

    }
    public event Action HasTrackDirectionChanged;
    private bool isTrackDirectionClockwise = true;
    public bool IsTrackDirectionClockwise
    {
        get { return isTrackDirectionClockwise; }
        set
        {
            // if (StartPiece)
            // {
            //     StartPiece.IsFacingForward = value;
            // }
            if (isTrackDirectionClockwise != value)
            {
                foreach (TrackPiece piece in track)
                {
                    piece.IsFacingForward = !piece.IsFacingForward;
                }
                isTrackDirectionClockwise = value;
                HasTrackDirectionChanged?.Invoke();
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
    private List<TrackPiece> track = new List<TrackPiece>();
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public bool IsCircular { get; private set; }
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
            // StartPiece.IsFacingForward = !StartPiece.IsFacingForward;
            StartPiece = grid.GetPiece(InputManager.Instance.MousePosition);
            SelectStart = false;
        }
    }
    public void ToggleTrackDirection()
    {
        IsTrackDirectionClockwise = !IsTrackDirectionClockwise;
    }
    public void ComputeTrack()
    {
        if (!StartPiece)
        {
            return;
        }
        ResetTrack();
        track.Add(StartPiece);
        // TrackPiece piece = GetNextTrackPiece(StartPiece, IsTrackDirectionClockwise);
        StartPiece.IsFacingForward = IsTrackDirectionClockwise;
        AddTrackPieceCheckpoints(StartPiece);
        TrackPiece piece = GetNextTrackPiece(StartPiece);
        while (piece != StartPiece && piece != null)
        {
            track.Add(piece);
            // piece = GetCurrentNextTrackPiece(piece);
            piece.IsFacingForward = DetermineTrackPieceFacingForward(piece);
            AddTrackPieceCheckpoints(piece);
            piece = GetNextTrackPiece(piece);
        }
        IsCircular = piece == StartPiece;
        ProcessCheckpoints();
    }
    private void ProcessCheckpoints()
    {
        Checkpoint firstCheckpoint = checkpoints[0];
        checkpoints.Remove(firstCheckpoint);
        if (IsCircular)
        {
            checkpoints.Add(firstCheckpoint);
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
    private TrackPiece GetNextTrackPiece(TrackPiece piece)
    {
        if (piece.IsFacingForward)
        {
            if (piece.IsStraight())
            {
                return grid.GetNeighbor(piece, piece.transform.forward);
            }
            return grid.GetNeighbor(piece, piece.transform.right);
        }
        return grid.GetNeighbor(piece, -piece.transform.forward);
    }
    private void AddTrackPieceCheckpoints(TrackPiece piece)
    {
        if (piece.IsFacingForward)
        {
            foreach (Checkpoint checkpoint in piece.GetCheckpoints())
            {
                checkpoint.transform.forward = checkpoint.InitialForward;
                checkpoints.Add(checkpoint);
            }
        }
        else
        {
            foreach (Checkpoint checkpoint in piece.GetCheckpoints().Reverse())
            {
                checkpoint.transform.forward = -checkpoint.InitialForward;
                checkpoints.Add(checkpoint);
            }
        }
    }
    private TrackPiece GetCurrentNextTrackPiece(TrackPiece piece)
    {
        TrackPiece previousPiece = track[track.Count - 2];
        TrackPiece backPiece = grid.GetNeighbor(piece, -piece.transform.forward);
        return GetNextTrackPiece(piece, previousPiece == backPiece);
    }
    private bool DetermineTrackPieceFacingForward(TrackPiece piece)
    {
        TrackPiece previousPiece = track[track.Count - 2];
        TrackPiece backPiece = grid.GetNeighbor(piece, -piece.transform.forward);
        return previousPiece == backPiece;
    }
    public bool IsValidStartPiece()
    {
        return StartPiece;
    }
    public bool HasValidTrackLength()
    {
        return track.Count > 3;
    }
    public TrackPiece GetRandomTrackPiece()
    {
        return track[UnityEngine.Random.Range(0, track.Count)];
    }
    public TrackPiece GetNextPieceFromTrack(TrackPiece trackPiece)
    {
        int index = track.IndexOf(trackPiece) + 1;
        if (index < 0 || index > track.Count - 1)
        {
            return null;
        }
        return track[index];
    }
}
