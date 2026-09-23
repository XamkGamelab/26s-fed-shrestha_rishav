using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; set; }

    [Header("Simulation")]
    [SerializeField] private GameObject _simulationManagerPrefab;

    private GameObject _simulationManager;
    private GameState _previousState;

    private void Awake()
    {
        CurrentState = GameState.MainMenu;
        _previousState = GameState.MainMenu;
    }

    private void OnEnable()
    {
        GameEventSystem.OnToggleTime += HandleToggleTime;
        GameEventSystem.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameEventSystem.OnToggleTime -= HandleToggleTime;
        GameEventSystem.OnGameStateChanged -= HandleGameStateChanged;
    }

    public static void ChangeState(GameState newState)
    {
        CurrentState = newState;
        GameEventSystem.InvokeGameStateChanged(newState);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing &&
            _previousState == GameState.MainMenu)
        {
            StartGameplay();
        }
        else if (state == GameState.MainMenu &&
                 (_previousState == GameState.Playing ||
                  _previousState == GameState.Paused))
        {
            StopGameplay();
        }

        _previousState = state;
    }

    private void StartGameplay()
    {
        _simulationManager = Instantiate(_simulationManagerPrefab);
    }

    private void StopGameplay()
    {
        if (_simulationManager != null)
        {
            Destroy(_simulationManager);
            _simulationManager = null;
        }
    }

    public void HandleToggleTime()
    {
        if (CurrentState == GameState.Playing)
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            ChangeState(GameState.MainMenu);
    }

    public void ChangetoMainMenu() => ChangeState(GameState.MainMenu);
    public void ChangetoPlaying() => ChangeState(GameState.Playing);
    public void ChangetoPaused() => ChangeState(GameState.Paused);
}

public enum GameState
{
    Playing,
    Paused,
    MainMenu
}