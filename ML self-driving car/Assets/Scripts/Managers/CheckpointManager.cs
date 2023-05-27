using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    private Track track;
    private List<Checkpoint> checkpoints;
    private int nextCheckpointIndex;
    private void Start()
    {
        checkpoints = track.GetCheckpoints();
    }
    public void PassedCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoints.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            Debug.Log("Correct");
            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Count;
        }
        else
        {
            Debug.Log("Wrong");
        }
    }
    private void Reset()
    {
        nextCheckpointIndex = 0;
    }
}
