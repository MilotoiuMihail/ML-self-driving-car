using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    private Track track;
    private List<Checkpoint> checkpoints;
    private Dictionary<Car, int> checkpointTracker = new Dictionary<Car, int>();
    private void Start()
    {
        checkpoints = track.GetCheckpoints();
    }
    public void PassedCheckpoint(Car car, Checkpoint checkpoint)
    {
        if (checkpoints.IndexOf(checkpoint) == checkpointTracker[car])
        {
            Debug.Log("Correct");
            checkpointTracker[car] = (checkpointTracker[car] + 1) % checkpoints.Count;
        }
        else
        {
            Debug.Log("Wrong");
        }
    }
    private void Reset()
    {
        foreach (Car car in checkpointTracker.Keys)
        {
            checkpointTracker[car] = 0;
        }
    }
}
