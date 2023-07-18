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
    [SerializeField] private CarPanel difficultyPanel;
    [SerializeField] private FinishScreen finishScreen;

    private void Start()
    {
        GameManager.Instance.RaceStart += DisplayPlaySettings;
        GameManager.Instance.RaceFinish += HidePlaySettings;
        GameManager.Instance.ExitPlayState += HandleExitPlay;
        InputManager.Instance.EscDown += ToggleView;
        HidePlaySettings();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.RaceStart -= DisplayPlaySettings;
        GameManager.Instance.RaceFinish -= HidePlaySettings;
        GameManager.Instance.ExitPlayState -= HandleExitPlay;
        InputManager.Instance.EscDown -= ToggleView;
    }
    private void ToggleView()
    {
        if (GameManager.Instance.IsGameState(GameState.PAUSED))
        {
            if (!ModalWindowSystem.Instance.IsVisible() && !difficultyPanel.IsVisible() && !finishScreen.IsVisible())
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
        settingsSection.SetActive(true);
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
        settingsSection.SetActive(true);
        optionsSection.SetActive(false);
    }
    public void ToOptions()
    {
        SetTitle(OptionsTitle);
        settingsSection.SetActive(false);
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
        if (!finishScreen.IsVisible())
        {
            GameManager.Instance.ChangeToPreviousState();
        }
        gameObject.SetActive(false);
    }
    private void DisplayPlaySettings()
    {
        playSection.gameObject.SetActive(true);
    }
    private void HidePlaySettings()
    {
        playSection.gameObject.SetActive(false);
    }
    private void HandleExitPlay()
    {
        HidePlaySettings();
        gameObject.SetActive(false);
    }
}
