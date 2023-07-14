using System.Collections;
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
        Debug.Log($"checkpoint: {NextCheckpointIndex}, laps: {LapCount}, laptime: {LapTimer.ElapsedTime}, totalTime:{GetTrackTime()}");
        NextCheckpointIndex = 0;
        LapCount = 0;
        LapTimes.Clear();
        if (!CarManager.Instance.BlockInput)
        {
            Debug.Log("Restart unblock");
            LapTimer.RestartTimer();
        }
        else
        {
            LapTimer.ResetTimer();
        }
        // Debug.Log($"checkpoint: {NextCheckpointIndex}, laps: {LapCount}, laptime: {LapTimer}, totalTime:{GetTrackTime()}");
    }
    public void CompleteLap()
    {
        // IncreaseLap();
        LapTimes.Add(LapTimer.ElapsedTime);
        Debug.Log("Added laptime");
        LapTimer.RestartTimer();
    }
    public void FinishRace()
    {
        Debug.Log("Finish race");
        if (GameManager.Instance.IsGameState(GameState.PLAY))
        {
            LapTimer.StopTimer();
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
