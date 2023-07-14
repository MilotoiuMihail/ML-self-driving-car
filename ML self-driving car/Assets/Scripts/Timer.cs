using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : IDisposable
{
    public float ElapsedTime { get; private set; }
    private Coroutine timerCoroutine;
    public Timer()
    {
        // Debug.Log("Created");
        CarManager.Instance.CarInputBlocked += StopTimer;
        CarManager.Instance.CarInputUnblocked += StartTimer;
        // GameManager.Instance.EnterViewState += StartTimer;
        GameManager.Instance.ExitViewState += StopTimer;
        // GameManager.Instance.EnterEditState += StopTimer;
        // GameManager.Instance.ExitEditState += ResetTimer;
    }
    public void Dispose()
    {
        // Debug.Log("Destroyed");
        CarManager.Instance.CarInputBlocked -= StopTimer;
        CarManager.Instance.CarInputUnblocked -= StartTimer;
        // GameManager.Instance.EnterViewState -= StartTimer;
        GameManager.Instance.ExitViewState -= StopTimer;
        // GameManager.Instance.EnterEditState -= StopTimer;
        // GameManager.Instance.ExitEditState -= ResetTimer;
    }
    private void StartTimer()
    {
        timerCoroutine = GameManager.Instance.StartCoroutine(TimerCoroutine());
    }
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            Debug.Log("Stop timer");
            GameManager.Instance.StopCoroutine(timerCoroutine);
        }
    }
    private IEnumerator TimerCoroutine()
    {
        Debug.Log("Start timer");
        while (true)
        {
            // Debug.Log($"{GetHashCode()}Timer ongoing:{ElapsedTime}");
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void RestartTimer()
    {
        ResetTimer();
        StartTimer();
    }
    public void ResetTimer()
    {
        ElapsedTime = 0;
        StopTimer();
    }
}
