using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public class UIPanelData
    {
        public UIPanel panel;
        public GameObject canvas;
    }
    public TextMeshProUGUI resourceText;
    public TextMeshProUGUI goldText;
    [SerializeField]
    public UIPanelData[] panels;

    private void OnEnable()
    {
        GameEventSystem.OnGameStateChanged += HandleGameStateChanged;
        GameEventSystem.OnGoldChanged += HandleGoldChanged;
        GameEventSystem.OnResourceChanged += HandleResourcesChanged;

    }
    private void OnDisable()
    {
        GameEventSystem.OnGameStateChanged -= HandleGameStateChanged;
        GameEventSystem.OnGoldChanged -= HandleGoldChanged;
        GameEventSystem.OnResourceChanged += HandleResourcesChanged;
    }

    private void HandleResourcesChanged(int iron, int wood, int wool, int stone, int milk)
    {
        resourceText.text = "Iron: " + iron + "\nWood: " + wood + "\nWool: " + wool + "\nStone: " + stone + "\nMilk: " +
                            milk;
    }
    private void HandleGoldChanged(int goldCoins)
    {
        goldText.text = "Gold: " + goldCoins;
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