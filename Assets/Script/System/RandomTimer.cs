using System;
using UnityEngine;

[System.Serializable]
public class RandomTimer
{
    [SerializeField] private Vector2 interval = new(5f, 15f);

    public event Action OnTimerElapsed;

    [SerializeField] private float remainingTime;

    public float RemainingTime => remainingTime;

    public void Initialize()
    {
        Reset();
    }

    public void Tick(float deltaTime)
    {
        remainingTime -= deltaTime;

        if (remainingTime > 0f)
            return;

        Reset();
        OnTimerElapsed?.Invoke();
    }

    public void Reset()
    {
        remainingTime = UnityEngine.Random.Range(
            interval.x,
            interval.y
        );
    }
}