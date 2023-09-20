using UnityEngine;
using TMPro;

public class LapTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text lapCount;
    [SerializeField] private TMP_Text lapTimeText;
    [SerializeField] private Track track;
    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += Hide;
        GameManager.Instance.ExitEditState += Show;
        CheckpointManager.Instance.CompletedLap += HandleLapCompletion;
        CheckpointManager.Instance.FinishedRace += HandleFinishedRace;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= Hide;
        GameManager.Instance.ExitEditState -= Show;
        CheckpointManager.Instance.CompletedLap -= HandleLapCompletion;
        CheckpointManager.Instance.FinishedRace -= HandleFinishedRace;
    }

    private void Start()
    {
        ResetLapText();
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
    private void Update()
    {

        UpdateLapTimeText();
    }
    private void UpdateLapTimeText()
    {
        float lapTime = CheckpointManager.Instance.Tracker[CarManager.Instance.Car.transform].LapTimer.ElapsedTime;
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
        UpdateLapText(tracker.LapCount);
    }
    private void HandleFinishedRace(Transform carTransform)
    {
        if (carTransform != CarManager.Instance.Car.transform || GameManager.Instance.IsGameState(GameState.PLAY))
        {
            return;
        }
    }
    private void UpdateLapText(int laps)
    {
        int maxLaps = track.IsCircular ? track.Laps : 1;
        lapCount.text = $"{Mathf.Min(laps, maxLaps)}/{maxLaps}";
    }
    public void ResetLapText()
    {
        UpdateLapText(1);
    }
}
