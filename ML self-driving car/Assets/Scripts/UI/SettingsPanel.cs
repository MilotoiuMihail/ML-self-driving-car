using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    private const string SettingsTitle = "Settings";
    private const string OptionsTitle = "Options";
    private const string FileOptionsTitle = "File";
    [SerializeField] private GameObject optionsSection;
    [SerializeField] private GameObject settingsSection;
    [SerializeField] private GameObject fileSection;
    [SerializeField] private GameObject mainSection;
    [SerializeField] private GameObject playSection;
    [SerializeField] private TMP_Text title;
    [SerializeField] private DifficultyPanel difficultyPanel;
    private void Start()
    {
        InputManager.Instance.EscDown += ToggleView;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        InputManager.Instance.EscDown -= ToggleView;
    }
    private void ToggleView()
    {
        if (GameManager.Instance.IsGameState(GameState.PAUSED))
        {
            if (!ModalWindowSystem.Instance.IsVisible() && !difficultyPanel.IsVisible())
            {
                Hide();
            }
        }
        else
        {
            Show();
        }
    }
    public void Show()
    {
        GameManager.Instance.ChangeToPausedState();
        gameObject.SetActive(true);
        SetTitle(SettingsTitle);
        mainSection.SetActive(true);
        optionsSection.SetActive(false);
        fileSection.SetActive(false);
    }
    public void FileBack()
    {
        SetTitle(OptionsTitle);
        optionsSection.SetActive(true);
        fileSection.SetActive(false);
    }
    public void OptionsBack()
    {
        SetTitle(SettingsTitle);
        mainSection.SetActive(true);
        optionsSection.SetActive(false);
    }
    public void ToOptions()
    {
        SetTitle(OptionsTitle);
        mainSection.SetActive(false);
        optionsSection.SetActive(true);
    }
    public void ToFileOptions()
    {
        SetTitle(FileOptionsTitle);
        optionsSection.SetActive(false);
        fileSection.SetActive(true);
    }
    private void SetTitle(string text)
    {
        title.text = text;
    }
    public void Hide()
    {
        GameManager.Instance.ChangeToPreviousState();
        gameObject.SetActive(false);
    }
}
