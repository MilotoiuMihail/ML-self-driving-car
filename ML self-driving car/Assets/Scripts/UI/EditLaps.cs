using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EditLaps : MonoBehaviour
{
    [SerializeField] private Track track;
    [SerializeField] private TMP_Text lapCount;
    private void OnEnable()
    {
        UpdateLapCount();
    }
    private void OnDisable()
    {
        // GameManager.Instance.EnterEditState -= UpdateLapCount;
    }
    public void IncreaseLapCount()
    {
        track.Laps += 1;
        UpdateLapCount();
    }
    public void DecreaseLapCount()
    {
        track.Laps -= 1;
        UpdateLapCount();
    }
    private void UpdateLapCount()
    {
        lapCount.text = track.Laps.ToString();
    }
}
