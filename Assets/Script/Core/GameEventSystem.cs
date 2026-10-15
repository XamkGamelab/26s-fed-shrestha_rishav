using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnBackPressed;
    public static event System.Action<bool> TogglePauseGame;
    public static event System.Action<bool> OnBuildModeToggled;
    public static event Action<int,int,int,int,int> OnResourceChanged;
    public static event Action<int> OnGoldChanged;
    public static event Action OnResetCursor;
    public static event Action ToggleGameTime;
    public static event Action Select;


    public static void InvokeBuildModeToggled(bool isActive)
    {
        OnBuildModeToggled?.Invoke(isActive);
    }
    public static void InvokeResourceChanged(int iron,int wood,int wool,int stone,int milk)
    {
        OnResourceChanged?.Invoke(iron,wood,wool,stone,milk);
    }
    public static void InvokeGoldChanged(int gold)
    {
        OnGoldChanged?.Invoke(gold);
    }
    public static void InvokeToggleTime()
    {
        ToggleGameTime?.Invoke();
    }
    public static void InvokeSelect()
    {
        Select?.Invoke();
    }
    public static void InvokeBackButtonPress()
    {
        OnBackPressed?.Invoke();
    }
    public static void InvokeResetCursor()
    {
        OnResetCursor?.Invoke();
    }
    public static void InvokeGameStateChanged(GameState state)
    {
        OnGameStateChanged?.Invoke(state);
    }
    public static void InvokeTogglePause()
    {
        TogglePauseGame?.Invoke(true);
    }
    public static void InvokeToggleUnPause()
    {
        TogglePauseGame?.Invoke(false);
    }
}
