using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PositionTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text playerPosition;
    private void OnEnable()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed += HandleCheckpointPassed;
    }
    private void OnDisable()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed -= HandleCheckpointPassed;
    }
    private void Start()
    {
        GameManager.Instance.EnterPlayState += Show;
        GameManager.Instance.ExitPlayState += Hide;
        Hide();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterPlayState -= Show;
        GameManager.Instance.ExitPlayState -= Hide;
    }
    private void Show()
    {
        // playerPosition.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        // playerPosition.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void UpdatePositionText(int currentPosition)
    {
        int totalPositions = CheckpointManager.Instance.Tracker.Count;
        playerPosition.text = $"{currentPosition}/{totalPositions}";
    }
    private void HandleCheckpointPassed(Transform carTransform)
    {
        var sortedTrackers = CheckpointManager.Instance.Tracker.OrderBy(tracker => tracker.Value.LapCount)
        .ThenBy(tracker => tracker.Value.NextCheckpointIndex)
        .ToDictionary(tracker => tracker.Key, tracker => tracker.Value);
        Debug.Log(carTransform.name);
        int position = 1;
        foreach (var tracker in sortedTrackers)
        {
            if (tracker.Key == CarManager.Instance.Car.transform && tracker.Value.Position != position)
            {
                Debug.Log($"{carTransform.name}:{position}");
                UpdatePositionText(position);
            }
            tracker.Value.Position = position;
            position += 1;
        }
    }
}
