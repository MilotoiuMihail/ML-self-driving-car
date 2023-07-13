using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text position;
    [SerializeField] private TMP_Text suffix;
    [SerializeField] private TMP_Text trackTime;
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
        UpdateTrackTimeText();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }
    private void UpdateTrackTimeText()
    {
        float TotalTime = CheckpointManager.Instance.Tracker[CarManager.Instance.Car.transform].GetTrackTime();
        float milliseconds = Mathf.Floor(TotalTime * 1000) % 1000;
        float seconds = Mathf.Floor(TotalTime) % 60;
        float minutes = Mathf.Floor(TotalTime / 60);

        trackTime.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
