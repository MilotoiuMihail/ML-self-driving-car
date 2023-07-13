using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackerData
{
    public int NextCheckpointIndex { get; set; }
    public int LapCount { get; private set; }
    public List<float> LapTimes { get; private set; } = new List<float>();
    public int Position { get; set; }

    public void Reset()
    {
        NextCheckpointIndex = 0;
        LapCount = 0;
        LapTimes.Clear();
    }
    public void CompleteLap()
    {
        LapCount += 1;
    }
    public float GetTrackTime()
    {
        return LapTimes.Sum();
    }
}
