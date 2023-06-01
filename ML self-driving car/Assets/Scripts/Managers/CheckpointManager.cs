using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [SerializeField] private Transform carsParent;
    [SerializeField] private Track track;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private Dictionary<Transform, int> checkpointTracker = new Dictionary<Transform, int>();
    public event Action<Transform> CorrectCheckpointPassed;
    public event Action<Transform> WrongCheckpointPassed;
    protected override void Awake()
    {
        base.Awake();
        foreach (CarAgent car in carsParent.GetComponentsInChildren<CarAgent>())
        {
            ResetCheckpoint(car.transform);
        }
    }
    private void Start()
    {
        checkpoints = track.GetCheckpoints();
    }
    public void PassedCheckpoint(Transform carTransform, Checkpoint checkpoint)
    {
        if (checkpoints.IndexOf(checkpoint) == checkpointTracker[carTransform])
        {
            checkpointTracker[carTransform] = (checkpointTracker[carTransform] + 1) % checkpoints.Count;
            CorrectCheckpointPassed?.Invoke(carTransform);
        }
        else
        {
            WrongCheckpointPassed?.Invoke(carTransform);
        }
    }
    public Checkpoint GetNextCheckpoint(Transform carTransform)
    {
        return checkpoints[checkpointTracker[carTransform]];
    }
    public void ResetCheckpoint(Transform carTransform)
    {
        //until i resolve car position on start piece
        checkpointTracker[carTransform] = 1;
    }
}
