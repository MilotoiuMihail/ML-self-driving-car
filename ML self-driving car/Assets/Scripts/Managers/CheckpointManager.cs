using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] private Transform carsParent;
    [SerializeField] private Track track;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public Dictionary<Transform, TrackerData> Tracker { get; private set; } = new Dictionary<Transform, TrackerData>();
    public event Action<Transform> CorrectCheckpointPassed;
    public event Action<Transform> WrongCheckpointPassed;
    public event Action<Transform> CompletedLap;
    public event Action<Transform> FinishedRace;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private TMP_Text wrongWay;
    private void OnEnable()
    {
        GameManager.Instance.EnterPlayState += ClearTrackers;
        GameManager.Instance.ExitPlayState += ClearTrackers;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterPlayState -= ClearTrackers;
        GameManager.Instance.ExitPlayState -= ClearTrackers;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void ClearTrackers()
    {
        foreach (TrackerData tracker in Tracker.Values)
        {
            tracker.LapTimer.Dispose();
        }
        Tracker.Clear();
    }
    private void Start()
    {
        checkpoints = track.GetCheckpoints();
        HideWrongWay();
    }
    public void ShowWrongWay()
    {
        wrongWay.gameObject.SetActive(true);
    }
    public void HideWrongWay()
    {
        wrongWay.gameObject.SetActive(false);
    }
    public void PassedCheckpoint(Car car, Checkpoint checkpoint)
    {
        Transform carTransform = car.transform;
        int carCurrentCheckpointIndex = Tracker[carTransform].NextCheckpointIndex;
        if (checkpoints.IndexOf(checkpoint) == carCurrentCheckpointIndex)
        {
            CorrectCheckpointPassed?.Invoke(carTransform);
            Tracker[carTransform].NextCheckpointIndex = GetNextIndex(carCurrentCheckpointIndex);
            if (!track.IsCircular && Tracker[carTransform].NextCheckpointIndex == 0)
            {

                Tracker[carTransform].IncreaseLap();
                OnCompletedLap(carTransform);
                OnFinishRace(carTransform);
                return;
            }
            if (carCurrentCheckpointIndex != 0)
            {
                return;
            }
            Tracker[carTransform].IncreaseLap();
            int currentLap = Tracker[carTransform].LapCount;
            if (currentLap == 1)
            {
                return;
            }
            OnCompletedLap(carTransform);
            if (currentLap > track.Laps)
            {
                OnFinishRace(carTransform);
            }
        }
        else
        {
            WrongCheckpointPassed?.Invoke(carTransform);
        }
    }
    private int GetNextIndex(int index)
    {
        return (index + 1) % checkpoints.Count;
    }
    private void OnFinishRace(Transform carTransform)
    {
        Tracker[carTransform].FinishRace();
        FinishedRace?.Invoke(carTransform);
    }
    private void OnCompletedLap(Transform carTransform)
    {
        Tracker[carTransform].CompleteLap();
        CompletedLap?.Invoke(carTransform);
    }
    public Checkpoint GetNextCheckpoint(Transform carTransform)
    {
        return checkpoints.Count > 0 ? checkpoints[GetNextIndex(Tracker[carTransform].NextCheckpointIndex)] : null;
    }
    public Checkpoint GetCurrentCheckpoint(Transform carTransform)
    {
        return checkpoints.Count > 0 ? checkpoints[Tracker[carTransform].NextCheckpointIndex] : null;
    }
    public List<Checkpoint> GetNextCheckpoints(Transform carTransform, int n)
    {
        List<Checkpoint> nextCheckpoints = new List<Checkpoint>();

        int currentCheckpointIndex = Tracker[carTransform].NextCheckpointIndex;
        int totalCheckpoints = checkpoints.Count;

        int remainingCheckpoints = totalCheckpoints - currentCheckpointIndex;
        int checkpointsToAdd = track.IsCircular ? n : Mathf.Min(n, remainingCheckpoints);

        for (int i = 0; i < checkpointsToAdd; i++)
        {
            int nextIndex = (currentCheckpointIndex + i) % totalCheckpoints;
            nextCheckpoints.Add(checkpoints[nextIndex]);
        }

        return nextCheckpoints;
    }
    public void ResetProgress(Transform carTransform)
    {
        if (!Tracker.ContainsKey(carTransform))
        {
            Tracker.Add(carTransform, new TrackerData());
        }
        Tracker[carTransform].Reset();
    }
    public int GetCheckpointIndex(Checkpoint checkpoint)
    {
        return checkpoints.IndexOf(checkpoint);
    }
    public int GetCheckpointsCount()
    {
        return checkpoints.Count;
    }
    public bool HasFinishedRace(Transform carTransform)
    {
        return Tracker.ContainsKey(carTransform) && Tracker[carTransform].LapCount > (track.IsCircular ? track.Laps : 1);
    }
}
