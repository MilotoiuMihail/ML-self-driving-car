using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LapTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text lapCount;
    [SerializeField] private TMP_Text lapTimeText;
    [SerializeField] private Track track;
    private float lapTime;
    private Coroutine timerCoroutine;
    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += Hide;
        GameManager.Instance.ExitEditState += Show;
        CheckpointManager.Instance.CompletedLap += HandleLapCompletion;
        GameManager.Instance.EnterViewState += ResetAll;
        GameManager.Instance.RaceStart += ResetAll;
        CarManager.Instance.CarInputBlocked += StopTimer;
        CarManager.Instance.CarInputUnblocked += StartTimer;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= Hide;
        GameManager.Instance.ExitEditState -= Show;
        CheckpointManager.Instance.CompletedLap -= HandleLapCompletion;
        GameManager.Instance.EnterViewState -= ResetAll;
        GameManager.Instance.RaceStart -= ResetAll;
        CarManager.Instance.CarInputBlocked -= StopTimer;
        CarManager.Instance.CarInputUnblocked -= StartTimer;
    }

    private void Start()
    {
        UpdateLapText(1);
        ResetTimer();
    }
    private void Show()
    {
        lapCount.transform.parent.gameObject.SetActive(true);
        lapTimeText.gameObject.SetActive(true);
    }
    private void Hide()
    {
        lapCount.transform.parent.gameObject.SetActive(false);
        lapTimeText.gameObject.SetActive(false);
    }
    private void ResetTimer()
    {
        ResetLapTime();
        StartTimer();
    }
    private void StartTimer()
    {
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            lapTime += Time.deltaTime;
            UpdateLapTimeText();
            yield return null;
        }
    }
    private void UpdateLapTimeText()
    {
        float milliseconds = Mathf.Floor(lapTime * 1000) % 1000;
        float seconds = Mathf.Floor(lapTime) % 60;
        float minutes = Mathf.Floor(lapTime / 60);

        lapTimeText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
    private void HandleLapCompletion(Transform carTransform)
    {
        if (carTransform != CarManager.Instance.Car.transform)
        {
            return;
        }
        TrackerData tracker = CheckpointManager.Instance.Tracker[carTransform];
        tracker.LapTimes.Add(lapTime);
        ResetLapTime();
        UpdateLapText(tracker.LapCount);
    }
    private void UpdateLapText(int laps)
    {
        int maxLaps = track.IsCircular ? track.Laps : 1;
        lapCount.text = $"{Mathf.Min(laps, maxLaps)}/{maxLaps}";
    }
    private void ResetAll()
    {
        ResetLapTime();
        UpdateLapText(1);
    }
    private void ResetLapTime()
    {
        lapTime = 0;
        UpdateLapTimeText();
    }
}
