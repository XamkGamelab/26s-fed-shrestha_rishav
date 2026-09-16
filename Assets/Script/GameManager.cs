using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; set; }

    private void Awake()
    {
        CurrentState = GameState.MainMenu;
    }

    public static void ChangeState(GameState newState)
    {
        CurrentState = newState;
        GameEventSystem.InvokeGameStateChanged(newState);
    }

    public void OnEnable()
    {
        GameEventSystem.OnToggleTime += HandleToggleTime;
    }

    public void OnDisable()
    {
        GameEventSystem.OnToggleTime -= HandleToggleTime;
    }

    public void HandleToggleTime()
    {
        if(CurrentState == GameState.Playing)
            ChangeState(GameState.Paused);
        else if(CurrentState == GameState.Paused)
            ChangeState(GameState.MainMenu);
    }
    public void ChangetoMainMenu()=> ChangeState(GameState.MainMenu);
    public void ChangetoPlaying()=> ChangeState(GameState.Playing);
    public void ChangetoPaused()=> ChangeState(GameState.Paused);
}
  

public enum GameState
{
    Playing,
    Paused,
    MainMenu
}