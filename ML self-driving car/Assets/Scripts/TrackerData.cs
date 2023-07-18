using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrackerData
{
    public int NextCheckpointIndex { get; set; }
    public int LapCount { get; private set; }
    public List<float> LapTimes { get; private set; } = new List<float>();
    public Timer LapTimer { get; private set; } = new Timer();

    public void Reset()
    {
        NextCheckpointIndex = 0;
        LapCount = 0;
        LapTimes.Clear();
        if (!CarManager.Instance.BlockInput)
        {
            LapTimer.RestartTimer();
        }
        else
        {
            LapTimer.ResetTimer();
        }
    }
    public void CompleteLap()
    {
        LapTimes.Add(LapTimer.ElapsedTime);
        LapTimer.RestartTimer();
    }
    public void FinishRace()
    {
        if (GameManager.Instance.IsGameState(GameState.PLAY))
        {
            LapTimer.ResetTimer();
        }
    }
    public void IncreaseLap()
    {
        LapCount += 1;
    }
    public float GetTrackTime()
    {
        return LapTimes.Sum();
    }
}
