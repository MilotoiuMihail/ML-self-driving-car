using UnityEngine;
using TMPro;

public class FinishScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text carNames;
    [SerializeField] private TMP_Text trackTimes;
    private void OnEnable()
    {
        GameManager.Instance.ExitPlayState += Hide;
        GameManager.Instance.RaceStart += Hide;
    }
    private void OnDisable()
    {
        GameManager.Instance.ExitPlayState -= Hide;
        GameManager.Instance.RaceStart -= Hide;
    }
    private void Start()
    {
        GameManager.Instance.RaceFinish += Show;
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        GameManager.Instance.RaceFinish -= Show;
    }
    private void Show()
    {
        gameObject.SetActive(true);
        GameManager.Instance.ChangeToPausedState();
        DecideWin();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }
    private string GetTrackTimeText(float totalTime)
    {
        float milliseconds = Mathf.Floor(totalTime * 1000) % 1000;
        float seconds = Mathf.Floor(totalTime) % 60;
        float minutes = Mathf.Floor(totalTime / 60);

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    private void DecideWin()
    {
        TrackerData playerTracker = CheckpointManager.Instance.Tracker[CarManager.Instance.PlayerCar.transform];
        TrackerData npcTracker = CheckpointManager.Instance.Tracker[CarManager.Instance.Npc.transform];
        float playerTime = playerTracker.GetTrackTime();
        float npcTime = npcTracker.GetTrackTime();
        if (playerTracker.LapCount != npcTracker.LapCount)
        {
            npcTime += npcTracker.LapTimer.ElapsedTime;
            title.text = "Won";
            carNames.text = $"Player:{System.Environment.NewLine}Npc:";
            trackTimes.text = $"{GetTrackTimeText(playerTime)}{System.Environment.NewLine}DNF";
        }
        else
        {
            if (playerTime > npcTime)
            {
                title.text = "Lost";
                carNames.text = $"Npc:{System.Environment.NewLine}Player:";
                trackTimes.text = $"{GetTrackTimeText(npcTime)}{System.Environment.NewLine}{GetTrackTimeText(playerTime)}";
            }
            else
            {
                title.text = "Won";
                carNames.text = $"Player:{System.Environment.NewLine}Npc:";
                trackTimes.text = $"{GetTrackTimeText(playerTime)}{System.Environment.NewLine}{GetTrackTimeText(npcTime)}";
            }
        }
    }
}
