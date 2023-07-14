using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private void OnEnable()
    {
        // GameManager.Instance.RaceStart += ResetProgressAll;
        GameManager.Instance.EnterPlayState += ClearTrackers;
        GameManager.Instance.ExitPlayState += ClearTrackers;
    }
    private void OnDisable()
    {
        // GameManager.Instance.RaceStart -= ResetProgressAll;
        GameManager.Instance.EnterPlayState -= ClearTrackers;
        GameManager.Instance.ExitPlayState -= ClearTrackers;
    }
    protected override void Awake()
    {
        base.Awake();
        // ResetProgressAll();
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
    }
    private void ResetProgressAll()
    {
        foreach (Car car in carsParent.GetComponentsInChildren<Car>())
        {
            ResetProgress(car.transform);
        }
    }
    public void PassedCheckpoint(Transform carTransform, Checkpoint checkpoint)
    {
        int carCurrentCheckpointIndex = Tracker[carTransform].NextCheckpointIndex;
        if (checkpoints.IndexOf(checkpoint) == carCurrentCheckpointIndex)
        {
            Tracker[carTransform].NextCheckpointIndex = (carCurrentCheckpointIndex + 1) % checkpoints.Count;
            CorrectCheckpointPassed?.Invoke(carTransform);
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
    private void OnFinishRace(Transform carTransform)
    {
        Tracker[carTransform].FinishRace();
        Debug.Log(carTransform.name);
        FinishedRace?.Invoke(carTransform);
        // ResetProgress(carTransform);
    }
    private void OnCompletedLap(Transform carTransform)
    {
        Tracker[carTransform].CompleteLap();
        CompletedLap?.Invoke(carTransform);
    }
    public Checkpoint GetNextCheckpoint(Transform carTransform)
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
