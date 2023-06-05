using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] private Transform carsParent;
    [SerializeField] private Track track;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public Dictionary<Transform, int> CheckpointTracker { get; private set; } = new Dictionary<Transform, int>();
    public Dictionary<Transform, int> LapTracker { get; private set; } = new Dictionary<Transform, int>();
    [SerializeField] private int laps;
    public event Action<Transform> CorrectCheckpointPassed;
    public event Action<Transform, int> WrongCheckpointPassed;
    public event Action<Transform> CompletedLap;
    public event Action<Transform> FinishedRace;
    protected override void Awake()
    {
        base.Awake();
        foreach (CarAgent car in carsParent.GetComponentsInChildren<CarAgent>())
        {
            ResetProgress(car.transform);
        }
    }
    private void Start()
    {
        checkpoints = track.GetCheckpoints();
    }
    public void PassedCheckpoint(Transform carTransform, Checkpoint checkpoint)
    {
        int carCurrentCheckpointIndex = CheckpointTracker[carTransform];
        if (checkpoints.IndexOf(checkpoint) == carCurrentCheckpointIndex)
        {
            CheckpointTracker[carTransform] = (carCurrentCheckpointIndex + 1) % checkpoints.Count;
            CorrectCheckpointPassed?.Invoke(carTransform);
            if (!track.IsCircular && CheckpointTracker[carTransform] == 0)
            {
                CompletedLap?.Invoke(carTransform);
                FinishedRace?.Invoke(carTransform);
                return;
            }
            if (carCurrentCheckpointIndex != 0)
            {
                return;
            }
            LapTracker[carTransform] += 1;
            int currentLap = LapTracker[carTransform];
            if (currentLap == 1)
            {
                return;
            }
            CompletedLap?.Invoke(carTransform);
            if (currentLap > laps)
            {
                FinishedRace?.Invoke(carTransform);
            }
        }
        else
        {
            WrongCheckpointPassed?.Invoke(carTransform, checkpoints.IndexOf(checkpoint));
        }
    }
    public Checkpoint GetNextCheckpoint(Transform carTransform)
    {
        return checkpoints[CheckpointTracker[carTransform]];
    }
    public void ResetProgress(Transform carTransform)
    {
        CheckpointTracker[carTransform] = 0;
        LapTracker[carTransform] = 0;
    }
    public int GetCheckpointIndex(Checkpoint checkpoint)
    {
        return checkpoints.IndexOf(checkpoint);
    }
    public int GetCheckpointsCount()
    {
        return checkpoints.Count;
    }
}
