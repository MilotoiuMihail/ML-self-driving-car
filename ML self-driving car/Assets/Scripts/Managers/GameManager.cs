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
    public event Action EnterPlayState;
    public event Action ExitPlayState;
    public event Action ExitViewState;
    public event Action ExitEditState;
    public event Action ExitPausedState;
    public event Action RaceStart;
    public event Action RaceFinish;
    private void Start()
    {
        CheckpointManager.Instance.FinishedRace += OnRaceFinish;
    }
    private void OnDestroy()
    {
        CheckpointManager.Instance.FinishedRace -= OnRaceFinish;
    }
    protected override void Awake()
    {
        base.Awake();
        ChangeGameState(GameState.VIEW);
    }

    public void ChangeGameState(GameState state)
    {
        if (currentState == state)
        {
            return;
        }
        if (state != GameState.PAUSED)
        {
            HandleBeforeStateChange();
        }
        previousState = currentState;
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
                Debug.Log("exit view");
                ExitViewState?.Invoke();
                break;
            case GameState.EDIT:
                Debug.Log("exit edit");
                ExitEditState?.Invoke();
                break;
            case GameState.PLAY:
                Debug.Log("exit play");
                ExitPlayState?.Invoke();
                break;
            case GameState.PAUSED:
                Debug.Log("exit pause");
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
                Debug.Log("enter view");
                EnterViewState?.Invoke();
                break;
            case GameState.EDIT:
                Debug.Log("enter edit");
                EnterEditState?.Invoke();
                break;
            case GameState.PLAY:
                Debug.Log("enter play");
                EnterPlayState?.Invoke();
                OnRaceStart();
                break;
            case GameState.PAUSED:
                Debug.Log("enter pause");
                EnterPausedState?.Invoke();
                break;
            default:
                Debug.LogWarning($"State {currentState.ToString()} has no 'after' event ");
                break;
        }
    }
    private void OnRaceFinish(Transform carTransform)
    {
        if (!GameManager.Instance.IsGameState(GameState.PLAY) || carTransform != CarManager.Instance.Car.transform)
        {
            return;
        }
        RaceFinish?.Invoke();
    }
    public bool IsGameState(GameState gameState)
    {
        return currentState == gameState;
    }
    public bool IsPreviousGameState(GameState gameState)
    {
        return previousState == gameState;
    }
    public void OnRaceStart()
    {
        RaceStart?.Invoke();
    }
    // used by buttons in inspector
    public void ExitApplication()
    {
        Application.Quit();
    }
    public void ChangeToViewState() => ChangeGameState(GameState.VIEW);
    public void ChangeToEditState() => ChangeGameState(GameState.EDIT);
    public void ChangeToPlayState() => ChangeGameState(GameState.PLAY);
    public void ChangeToPausedState() => ChangeGameState(GameState.PAUSED);
    public void ChangeToPreviousState() => ChangeGameState(previousState);
}

[Serializable]
public enum GameState
{
    VIEW,
    EDIT,
    PLAY,
    PAUSED
}
