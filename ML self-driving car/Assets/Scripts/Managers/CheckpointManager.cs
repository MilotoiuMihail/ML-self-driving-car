using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] private Transform carsParent;
    [SerializeField] private Track track;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    public Dictionary<Transform, int> CheckpointTracker { get; set; } = new Dictionary<Transform, int>();
    private Dictionary<Transform, int> lapTracker = new Dictionary<Transform, int>();
    [SerializeField] private int laps;
    public event Action<Transform> CorrectCheckpointPassed;
    public event Action<Transform> WrongCheckpointPassed;
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
        int carNextCheckpointIndex = CheckpointTracker[carTransform];
        if (checkpoints.IndexOf(checkpoint) == carNextCheckpointIndex)
        {
            CheckpointTracker[carTransform] = (carNextCheckpointIndex + 1) % checkpoints.Count;
            CorrectCheckpointPassed?.Invoke(carTransform);
            if (checkpoints.IndexOf(checkpoint) != 0)
            {
                return;
            }
            lapTracker[carTransform] += 1;
            int currentLap = lapTracker[carTransform];
            if (currentLap == 1)
            {
                return;
            }
            if (currentLap > laps)
            {
                FinishedRace?.Invoke(carTransform);
            }
            else
            {
                CompletedLap?.Invoke(carTransform);
            }
        }
        else
        {
            WrongCheckpointPassed?.Invoke(carTransform);
        }
    }
    public Checkpoint GetNextCheckpoint(Transform carTransform)
    {
        return checkpoints[CheckpointTracker[carTransform]];
    }
    public void ResetProgress(Transform carTransform)
    {
        CheckpointTracker[carTransform] = 0;
        lapTracker[carTransform] = 0;
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
