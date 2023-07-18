using System.Collections;
using UnityEngine;
using System;

public class Timer : IDisposable
{
    public float ElapsedTime { get; private set; }
    private Coroutine timerCoroutine;
    public Timer()
    {
        CarManager.Instance.CarInputBlocked += StopTimer;
        CarManager.Instance.CarInputUnblocked += StartTimer;
        GameManager.Instance.ExitViewState += StopTimer;
    }
    public void Dispose()
    {
        CarManager.Instance.CarInputBlocked -= StopTimer;
        CarManager.Instance.CarInputUnblocked -= StartTimer;
        GameManager.Instance.ExitViewState -= StopTimer;
    }
    private void StartTimer()
    {
        timerCoroutine = GameManager.Instance.StartCoroutine(TimerCoroutine());
    }
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(timerCoroutine);
        }
    }
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
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
