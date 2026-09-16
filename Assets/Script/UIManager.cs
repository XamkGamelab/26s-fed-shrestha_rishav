using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class UIPanelData
    {
        public UIPanel panel;
        public GameObject canvas;
    }
    [SerializeField]
    public UIPanelData[] panels;

    private void OnEnable()
    {
        GameEventSystem.OnGameStateChanged += HandleGameStateChanged;

    }
    private void OnDisable()
    {
        GameEventSystem.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        UIPanel panelToShow = state switch
        {
            GameState.MainMenu => UIPanel.CanvasMainMenu,
            GameState.Playing => UIPanel.CanvasHUD,
            GameState.Paused => UIPanel.CanvasPause,
            _ => UIPanel.CanvasMainMenu
        };

        foreach (var panel in panels)
        {
            panel.canvas.SetActive(panel.panel == panelToShow);
        }
    }
    
}
public enum UIPanel
{
    CanvasMainMenu,
    CanvasHUD,
    CanvasPause
}