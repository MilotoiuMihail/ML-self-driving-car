using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ButtonEditMode : MonoBehaviour
{
    private const string TrackText = "Track";
    private const string ObstaclesText = "Obstacles";
    [SerializeField] private Transform trackButtons;
    private Toggle toggle;
    private TMP_Text textComponent;
    private async void OnEnable()
    {
        await System.Threading.Tasks.Task.Yield();
        GameManager.Instance.EnterEditState += SetTrackEditMode;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= SetTrackEditMode;
    }
    private void Awake()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
        toggle = GetComponent<Toggle>();
    }
    public void Switch(bool isEditingObstacles)
    {
        ToggleButtons(!isEditingObstacles);
        textComponent.text = isEditingObstacles ? TrackText : ObstaclesText;
    }
    private void ToggleButtons(bool isActive)
    {
        for (int i = 0; i < trackButtons.childCount; i++)
        {
            trackButtons.GetChild(i).gameObject.SetActive(isActive);
        }
    }
    private void SetTrackEditMode()
    {
        toggle.isOn = false;
    }
}
