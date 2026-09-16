using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnToggleTime;
    
    public static void InvokeToggleTime()
    {
        OnToggleTime?.Invoke();
    }
    public static void InvokeGameStateChanged(GameState state)
    {
        OnGameStateChanged?.Invoke(state);
    }
}
