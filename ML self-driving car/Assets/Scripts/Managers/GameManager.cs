using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    private GameState currentState = GameState.PAUSED;
    private GameState previousState;
    public event Action EnterViewState;
    public event Action EnterEditState;
    public event Action EnterPausedState;
    public event Action ExitViewState;
    public event Action ExitEditState;
    public event Action ExitPausedState;
    protected override void Awake()
    {
        base.Awake();
        ChangeGameState(GameState.VIEW);
    }
    public void ChangeGameState(GameState state)
    {
        previousState = currentState;
        if (state != GameState.PAUSED)
        {
            HandleBeforeStateChange();
        }
        currentState = state;
        if (previousState != GameState.PAUSED)
        {
            HandleAfterStateChange();
        }
    }
    private void HandleBeforeStateChange()
    {
        switch (currentState)
        {
            case GameState.VIEW:
                ExitViewState?.Invoke();
                break;
            case GameState.EDIT:
                ExitEditState?.Invoke();
                break;
            case GameState.PAUSED:
                ExitPausedState?.Invoke();
                break;
            default:
                Debug.LogWarning($"State {currentState.ToString()} has no 'before' event ");
                break;
        }
    }
    private void HandleAfterStateChange()
    {
        switch (currentState)
        {
            case GameState.VIEW:
                EnterViewState?.Invoke();
                break;
            case GameState.EDIT:
                EnterEditState?.Invoke();
                break;
            case GameState.PAUSED:
                EnterPausedState?.Invoke();
                break;
            default:
                Debug.LogWarning($"State {currentState.ToString()} has no 'after' event ");
                break;
        }
    }
    public bool HasGameState(GameState state)
    {
        return currentState == state;
    }
    // used by buttons in inspector
    public void ExitApplication()
    {
        Application.Quit();
    }
    public void ChangeToViewState() => ChangeGameState(GameState.VIEW);
    public void ChangeToEditState() => ChangeGameState(GameState.EDIT);
    public void ChangeToPausedState() => ChangeGameState(GameState.PAUSED);
    public void ChangeToPreviousState() => ChangeGameState(previousState);
}

[Serializable]
public enum GameState
{
    VIEW,
    EDIT,
    PAUSED
}
