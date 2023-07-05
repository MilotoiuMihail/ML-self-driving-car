using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private ScalableTextContainer title;
    [SerializeField] private ScalableTextContainer body;
    private ModalButton[] buttons;
    private bool shouldPause;
    private const int SHOW_DELAY = 100;
    private async void OnEnable()
    {
        await System.Threading.Tasks.Task.Yield();
        InputManager.Instance.EscDown += Close;
    }
    private void OnDisable()
    {
        InputManager.Instance.EscDown -= Close;
    }
    private void Awake()
    {
        buttons = GetComponentsInChildren<ModalButton>();
        if (buttons.Length == 0)
        {
            Debug.LogWarning("No Buttons found in children");
        }
    }
    private void Start()
    {
        Close();
    }
    private bool IsInRange(int index)
    {
        return index >= 0 && index < buttons?.Length;
    }
    public int GetNoButtons()
    {
        return buttons.Length;
    }
    //used in inspector
    public void ButtonPressed(int index)
    {
        Close();
        if (!IsInRange(index))
        {
            Debug.LogWarning("Button index out of range");
            return;
        }
        buttons[index].OnInteract();
    }
    public void Close()
    {
        if (shouldPause)
        {
            GameManager.Instance.ChangeToPreviousState();
            shouldPause = false;
        }
        gameObject.SetActive(false);
    }
    public async void Show(ModalWindowData data)
    {
        SetTitle(data.Title);
        SetBody(data.Body);
        SetButtons(data.GetButtons());
        if (!GameManager.Instance.IsGameState(GameState.PAUSED))
        {
            shouldPause = true;
            GameManager.Instance.ChangeToPausedState();
        }
        await System.Threading.Tasks.Task.Delay(SHOW_DELAY);
        gameObject.SetActive(true);
    }
    private void SetTitle(string text)
    {
        title.SetText(text);
        title.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    private void SetBody(string text)
    {
        body.SetText(text);
        body.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    private void SetButtons(ModalButtonData[] buttonsData)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            SetButton(buttons[i], i < buttonsData.Length ? buttonsData[i] : new ModalButtonData());
        }
    }

    private void SetButton(ModalButton button, ModalButtonData buttonData)
    {
        button.SetText(buttonData.Text);
        button.SetColor(buttonData.Color);
        button.SetInteractAction(buttonData.ActionEvent);
        button.gameObject.SetActive(!string.IsNullOrEmpty(buttonData.Text));
    }
}
