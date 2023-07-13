using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Track : MonoBehaviour
{
    private const int MinLaps = 1;
    private const int MaxLaps = 100;
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
    private int laps;
    public int Laps
    {
        get
        {
            return laps;
        }
        set
        {
            laps = Mathf.Clamp(value, MinLaps, MaxLaps);
        }
    }
    [SerializeField] private Grid grid;
    [SerializeField] private TrackEditor trackEditor;

    private void OnEnable()
    {
        trackEditor.PiecePlaced += SelectStartPiece;
        trackEditor.PieceRemoved += OnPieceRemoved;
    }
    private void OnDisable()
    {
        trackEditor.PiecePlaced -= SelectStartPiece;
        trackEditor.PieceRemoved -= OnPieceRemoved;
    }
    private void OnPieceRemoved(TrackPiece piece)
    {
        if (piece != StartPiece)
        {
            return;
        }
        StartPiece = null;
    }
    public List<Checkpoint> GetCheckpoints()
    {
        return checkpoints;
    }
    public Checkpoint GetLastCheckpoint()
    {
        return checkpoints[checkpoints.Count - 1];
    }
    public Checkpoint GetFirstCheckpoint()
    {
        return checkpoints[0];
    }
    public void SelectStartPiece()
    {
        if (!StartPiece || SelectStart)
        {
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

        StartPiece.IsFacingForward = IsTrackDirectionClockwise;
        AddTrackPieceWithCheckpoints(StartPiece);
        TrackPiece nextPiece = GetNextTrackPiece(StartPiece);
        while (IsValidNextPiece(nextPiece))
        {
            AddTrackPieceWithCheckpoints(nextPiece);
            nextPiece = GetNextTrackPiece(nextPiece);
        }
        IsCircular = nextPiece == StartPiece;
        ProcessCheckpoints();
    }

    private bool IsValidNextPiece(TrackPiece nextPiece)
    {
        if (nextPiece == null || nextPiece == StartPiece)
        {
            return false;
        }
        nextPiece.IsFacingForward = DetermineTrackPieceFacingForward(nextPiece);
        if (nextPiece.IsFacingForward)
        {
            return true;
        }
        return IsConnectedToPreviousPiece(nextPiece);
    }
    private bool IsConnectedToPreviousPiece(TrackPiece piece)
    {
        piece.IsFacingForward = !piece.IsFacingForward;
        TrackPiece backTrackPiece = GetNextTrackPiece(piece);
        piece.IsFacingForward = !piece.IsFacingForward;
        return backTrackPiece == track[track.Count - 1];
    }
    private bool DetermineTrackPieceFacingForward(TrackPiece piece)
    {
        TrackPiece previousPiece = track[track.Count - 1];
        TrackPiece backPiece = grid.GetNeighbor(piece, -piece.transform.forward);
        return previousPiece == backPiece;
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
    private void AddTrackPieceWithCheckpoints(TrackPiece piece)
    {
        track.Add(piece);
        AddTrackPieceCheckpoints(piece);
    }
    private void AddTrackPieceCheckpoints(TrackPiece piece)
    {
        IEnumerable<Checkpoint> checkpointsToAdd = piece.GetCheckpoints();
        if (!piece.IsFacingForward)
        {
            checkpointsToAdd = checkpointsToAdd.Reverse();
        }

        foreach (Checkpoint checkpoint in checkpointsToAdd)
        {
            checkpoint.transform.forward = piece.IsFacingForward ? checkpoint.InitialForward : -checkpoint.InitialForward;
            checkpoints.Add(checkpoint);
        }
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
        return track.Count > 0 ? track[UnityEngine.Random.Range(0, track.Count)] : null;
    }

    public TrackData ToData()
    {
        TrackData trackData = new TrackData();
        trackData.IsTrackDirectionClockwise = IsTrackDirectionClockwise;
        trackData.StartPiece = StartPiece != null ? StartPiece.ToData() : null;
        trackData.Laps = Laps;
        return trackData;
    }
    public void Load(TrackData data)
    {
        if (data == null)
        {
            return;
        }
        IsTrackDirectionClockwise = data.IsTrackDirectionClockwise;
        Laps = data.Laps;
        StartPiece = data.StartPiece != null ? grid.GetPiece(data.StartPiece.Position) : null;
    }
}
