using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
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
            if (!ModalWindowSystem.Instance.IsVisible())
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
    }
    public void Hide()
    {
        GameManager.Instance.ChangeToPreviousState();
        gameObject.SetActive(false);
    }
}
