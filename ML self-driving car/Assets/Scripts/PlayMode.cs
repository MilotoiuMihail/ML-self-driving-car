using System.Collections;
using UnityEngine;
using TMPro;

public class PlayMode : MonoBehaviour
{
    private Coroutine timerCoroutine;
    [SerializeField] private TMP_Text countdown;
    private float timer;
    [SerializeField] private SettingsPanel settingsPanel;
    private void OnEnable()
    {
        GameManager.Instance.EnterPausedState += StopTimer;
        GameManager.Instance.ExitPausedState += StartRaceTimer;
        GameManager.Instance.ExitPlayState += StopTimer;
        StartRaceTimer();
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterPausedState -= StopTimer;
        GameManager.Instance.ExitPausedState -= StartRaceTimer;
        GameManager.Instance.ExitPlayState -= StopTimer;
    }
    private void Awake()
    {
        GameManager.Instance.EnterPlayState += Show;
        GameManager.Instance.ExitPlayState += Hide;
        HideCountdown();
        Hide();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterPlayState -= Show;
        GameManager.Instance.ExitPlayState -= Hide;
    }
    private void Show()
    {
        this.enabled = true;
    }
    private void Hide()
    {
        this.enabled = false;
    }
    private void UpdateCountdown()
    {
        timer = timer < 0 ? 0 : timer;
        countdown.text = timer.ToString("0");
    }
    public void StartTimer(float startTime)
    {
        ShowCountdown();
        timerCoroutine = StartCoroutine(TimerCoroutine(startTime));
    }
    public void StopTimer()
    {
        HideCountdown();
    }
    private IEnumerator TimerCoroutine(float startTime)
    {
        timer = startTime;
        UpdateCountdown();
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateCountdown();
            yield return null;
        }
        timer = 0;
        HideCountdown();
        CarManager.Instance.UnblockCarInput();
    }
    private void ShowCountdown()
    {
        countdown.gameObject.SetActive(true);
    }
    private void HideCountdown()
    {
        if (timerCoroutine != null && timer != 0)
        {
            StopCoroutine(timerCoroutine);
        }
        countdown.gameObject.SetActive(false);
    }
    private void StartRaceTimer()
    {
        CarManager.Instance.BlockCarInput();
        StartTimer(3);
    }
    public void PromptRestart()
    {
        ModalWindowData modalWindowData = ModalWindowSystem.Instance.SetTitle("Restart Race?")
                                .SetBody($"Do you want to restart the race?{System.Environment.NewLine}You will lose the current progress.")
                                .SetButton(0, "No", () => { }, ColorSystem.ColorPalette.Success)
                                .SetButton(1, "Restart", StartRace, ColorSystem.ColorPalette.Danger)
                                .BuildData();
        ModalWindowSystem.Instance.Show(modalWindowData);
    }
    public void StartRace()
    {
        settingsPanel.Hide();
        GameManager.Instance.OnRaceStart();
    }
    public void PromptReturnToMenu()
    {
        ModalWindowData modalWindowData = ModalWindowSystem.Instance.SetTitle("Return to Menu?")
                                .SetBody($"Do you want to return to the menu?")
                                .SetButton(0, "No", () => { }, ColorSystem.ColorPalette.Success)
                                .SetButton(1, "Return To Menu", ReturnToMenu, ColorSystem.ColorPalette.Danger)
                                .BuildData();
        ModalWindowSystem.Instance.Show(modalWindowData);
    }
    private void ReturnToMenu()
    {
        GameManager.Instance.ChangeToPreviousState();
        GameManager.Instance.ChangeToViewState();
    }
}
